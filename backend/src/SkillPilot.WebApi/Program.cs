using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SkillPilot.Application;
using SkillPilot.Infrastructure;
using SkillPilot.Infrastructure.Auth;
using SkillPilot.WebApi.Middleware;
using SkillPilot.WebApi.Swagger;

const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    // Enum(Status/Level/Category等)をJSON上は数値ではなく文字列名で表現する
    // (docs/api.mdで "Pending" のような文字列を返す前提としているため)。
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SkillPilot API", Version = "v1" });

    // ログイン後に発行されるJWTはhttpOnly Cookie(access_token)で受け渡す設計のため、
    // Swagger UI上でもBearerではなくCookie方式のセキュリティスキームとして表現する。
    options.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "access_token",
        Description = "POST /api/v1/auth/login のレスポンスで発行されるJWT(httpOnly Cookie)。",
    });
    options.OperationFilter<AuthorizeCheckOperationFilter>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// コンポジションルート: Application/Infrastructure層のDI登録をここでまとめて呼び出す。
// (この2行だけでUseCase・Repository・DbContext・AI連携がすべて登録される)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
{
    // 弱い既定値へフォールバックさせず、設定漏れを起動時に必ず気づける形にする。
    throw new InvalidOperationException(
        "Jwt:SigningKey が設定されていません。`dotnet user-secrets set \"Jwt:SigningKey\" \"...\"` " +
        "またはDocker/本番環境では環境変数 Jwt__SigningKey で設定してください。");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // 既定ではJwtBearerHandlerが "sub" 等の短いクレーム名をClaimTypes.*の長いURIに
        // 自動変換してしまう(DefaultInboundClaimTypeMap)。JwtTokenGeneratorが発行した
        // クレーム名(JwtRegisteredClaimNames.Sub)をそのまま読み取れるよう無効化する。
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
        };

        // JWTはhttpOnly Cookie("access_token")で受け渡す設計のため、Authorizationヘッダーではなく
        // Cookieからトークンを読み取る(docs/api.mdの認証方針を参照)。
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access_token", out var token))
                    context.Token = token;

                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddAuthorization(options =>
{
    // 既定を「認証必須」にし、公開エンドポイント([AllowAnonymous]、例: register/login)だけを
    // 個別に許可する(fail-closed。付け忘れたエンドポイントが誤って公開されることを防ぐ)。
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Cookie(JWT)を送受信するため必須
    });
});

var app = builder.Build();

// 想定外の例外(3層の例外設計における③)を最終的に捕捉するミドルウェア。
// パイプラインの最も外側に置き、以降のミドルウェア/Controllerで発生した例外をすべて拾う。
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
