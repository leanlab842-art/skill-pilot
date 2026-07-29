using SkillPilot.Application.Abstractions.Auth;
using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Results;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Application.Auth.Register;

/// <summary>ユーザー登録を行うUseCase。</summary>
public interface IRegisterUserUseCase
{
    /// <summary>新規ユーザーを登録する。</summary>
    Task<Result<RegisterUserResponse>> ExecuteAsync(RegisterUserRequest request, CancellationToken ct);
}

/// <summary>ユーザー登録のリクエスト。</summary>
/// <param name="Name">表示名。</param>
/// <param name="Email">メールアドレス。</param>
/// <param name="Password">生のパスワード。</param>
public sealed record RegisterUserRequest(string Name, string Email, string Password);

/// <summary>ユーザー登録のレスポンス。</summary>
/// <param name="UserId">作成されたユーザーId。</param>
/// <param name="Name">表示名。</param>
/// <param name="Email">メールアドレス。</param>
public sealed record RegisterUserResponse(Guid UserId, string Name, string Email);

/// <inheritdoc cref="IRegisterUserUseCase" />
public sealed class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<RegisterUserResponse>> ExecuteAsync(RegisterUserRequest request, CancellationToken ct)
    {
        Email email;
        try
        {
            email = Email.Create(request.Email);
        }
        catch (ArgumentException ex)
        {
            // Domainの不変条件違反(例外)を、Application境界でResultに変換する
            return Error.Validation("INVALID_EMAIL", ex.Message);
        }

        if (await _userRepository.ExistsByEmailAsync(email, ct))
            return Error.Conflict("EMAIL_ALREADY_REGISTERED", "このメールアドレスは既に登録されています。");

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User(request.Name, email, passwordHash);

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new RegisterUserResponse(user.Id, user.Name, user.Email.Value);
    }
}
