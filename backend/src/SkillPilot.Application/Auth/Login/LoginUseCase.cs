using SkillPilot.Application.Abstractions.Auth;
using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Results;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Application.Auth.Login;

/// <summary>ログインを行うUseCase。</summary>
public interface ILoginUseCase
{
    /// <summary>メールアドレスとパスワードでログインし、JWTアクセストークンを発行する。</summary>
    Task<Result<LoginResponse>> ExecuteAsync(LoginRequest request, CancellationToken ct);
}

/// <summary>ログインのリクエスト。</summary>
/// <param name="Email">メールアドレス。</param>
/// <param name="Password">生のパスワード。</param>
public sealed record LoginRequest(string Email, string Password);

/// <summary>ログインのレスポンス。</summary>
/// <param name="AccessToken">JWTアクセストークン。Controllerがこの値をhttpOnly Cookieに設定する。</param>
/// <param name="UserId">ログインしたユーザーId。</param>
/// <param name="Name">表示名。</param>
public sealed record LoginResponse(string AccessToken, Guid UserId, string Name);

/// <inheritdoc cref="ILoginUseCase" />
public sealed class LoginUseCase : ILoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        // メールアドレスが未登録か、パスワードが違うかをクライアントに区別させないため、
        // どちらの失敗でも同じエラーコード・メッセージを返す。
        var invalidCredentials = Error.Validation("INVALID_CREDENTIALS", "メールアドレスまたはパスワードが正しくありません。");

        Email email;
        try
        {
            email = Email.Create(request.Email);
        }
        catch (ArgumentException)
        {
            return invalidCredentials;
        }

        var user = await _userRepository.GetByEmailAsync(email, ct);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return invalidCredentials;

        var accessToken = _jwtTokenGenerator.Generate(user.Id);

        return new LoginResponse(accessToken, user.Id, user.Name);
    }
}
