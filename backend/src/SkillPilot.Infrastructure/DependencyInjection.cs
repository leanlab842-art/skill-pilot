using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SkillPilot.Application.Abstractions.Ai;
using SkillPilot.Application.Abstractions.Auth;
using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Infrastructure.Ai.Claude;
using SkillPilot.Infrastructure.Auth;
using SkillPilot.Infrastructure.Persistence;
using SkillPilot.Infrastructure.Persistence.Repositories;

namespace SkillPilot.Infrastructure;

/// <summary>Infrastructure層のサービスをDIコンテナへ登録する。</summary>
public static class DependencyInjection
{
    /// <summary>DbContext・Repository・認証・AI連携をまとめて登録する。</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SkillPilotDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSkillRepository, UserSkillRepository>();
        services.AddScoped<IJobAnalysisRepository, JobAnalysisRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        // v1はClaude APIのみをAI分析プロバイダとして登録する(OpenAIはrequirements.mdの
        // 「将来追加」に位置づけられており未実装)。IJobSkillAnalyzerの抽象を通しているため、
        // 将来OpenAiJobSkillAnalyzerを追加した際は、ここにAi:Providerの値に応じた分岐を足すだけでよい。
        services.Configure<ClaudeOptions>(configuration.GetSection("Ai:Claude"));
        services.AddHttpClient<IJobSkillAnalyzer, ClaudeJobSkillAnalyzer>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<ClaudeOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
