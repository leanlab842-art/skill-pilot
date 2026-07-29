using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Profile.GetProfile;

/// <summary>自分のプロフィールを取得するUseCase。</summary>
public interface IGetProfileUseCase
{
    Task<Result<ProfileDto>> ExecuteAsync(Guid userId, CancellationToken ct);
}

/// <inheritdoc cref="IGetProfileUseCase" />
public sealed class GetProfileUseCase : IGetProfileUseCase
{
    private readonly IUserRepository _userRepository;

    public GetProfileUseCase(IUserRepository userRepository) => _userRepository = userRepository;

    /// <inheritdoc />
    public async Task<Result<ProfileDto>> ExecuteAsync(Guid userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
            return Error.NotFound("USER_NOT_FOUND", "ユーザーが見つかりません。");

        return ProfileDto.FromEntity(user);
    }
}
