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

        Response.Cookies.Append(AccessTokenCookieName, result.Value.AccessToken, BuildCookieOptions(
            DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes)));

        return Ok(result.Value);
    }

    /// <summary>ログアウトする。アクセストークンのCookieを削除する。</summary>
    /// <remarks>
    /// ドメインロジックを持たない(Cookie削除のみ)ため、UseCaseを設けずController内で完結させる。
    /// </remarks>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        // Deleteは発行時と同じ属性(Path等)のCookieOptionsを指定しないとブラウザ側で
        // 削除対象と認識されないことがあるため、Login発行時と揃える。
        Response.Cookies.Delete(AccessTokenCookieName, BuildCookieOptions(DateTimeOffset.UnixEpoch));
        return NoContent();
    }

    private static CookieOptions BuildCookieOptions(DateTimeOffset expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
        Expires = expires,
    };
}
