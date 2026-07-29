using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SkillPilot.Application.Auth.Login;
using SkillPilot.Application.Auth.Register;
using SkillPilot.Infrastructure.Auth;
using SkillPilot.WebApi.Common;

namespace SkillPilot.WebApi.Controllers;

/// <summary>ユーザー登録・ログインを扱う。</summary>
[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private const string AccessTokenCookieName = "access_token";

    private readonly IRegisterUserUseCase _registerUserUseCase;
    private readonly ILoginUseCase _loginUseCase;
    private readonly JwtOptions _jwtOptions;

    public AuthController(
        IRegisterUserUseCase registerUserUseCase,
        ILoginUseCase loginUseCase,
        IOptions<JwtOptions> jwtOptions)
    {
        _registerUserUseCase = registerUserUseCase;
        _loginUseCase = loginUseCase;
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>新規ユーザーを登録する。</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var result = await _registerUserUseCase.ExecuteAsync(request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// メールアドレスとパスワードでログインする。成功時はJWTアクセストークンを
    /// httpOnly Cookieに設定する。
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _loginUseCase.ExecuteAsync(request, ct);
        if (!result.IsSuccess)
            return result.ToActionResult(this);

        Response.Cookies.Append(AccessTokenCookieName, result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
        });

        return Ok(result.Value);
    }
}
