using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Profile.UpdateProfile;

/// <summary>自分のプロフィールを更新するUseCase。</summary>
public interface IUpdateProfileUseCase
{
    Task<Result<ProfileDto>> ExecuteAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct);
}

/// <summary>プロフィール更新のリクエスト。</summary>
/// <param name="Name">新しい表示名。</param>
public sealed record UpdateProfileRequest(string Name);

/// <inheritdoc cref="IUpdateProfileUseCase" />
public sealed class UpdateProfileUseCase : IUpdateProfileUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<ProfileDto>> ExecuteAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Error.NotFound("USER_NOT_FOUND", "ユーザーが見つかりません。");

        user.UpdateProfile(request.Name);
        await _unitOfWork.SaveChangesAsync(ct);

        return ProfileDto.FromEntity(user);
    }
}
