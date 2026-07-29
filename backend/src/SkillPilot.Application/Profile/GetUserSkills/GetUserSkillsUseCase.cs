using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Profile.GetUserSkills;

/// <summary>自分の保有スキル一覧を取得するUseCase。</summary>
public interface IGetUserSkillsUseCase
{
    Task<Result<IReadOnlyList<UserSkillDto>>> ExecuteAsync(Guid userId, CancellationToken ct);
}

/// <inheritdoc cref="IGetUserSkillsUseCase" />
public sealed class GetUserSkillsUseCase : IGetUserSkillsUseCase
{
    private readonly IUserSkillRepository _userSkillRepository;

    public GetUserSkillsUseCase(IUserSkillRepository userSkillRepository) => _userSkillRepository = userSkillRepository;

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<UserSkillDto>>> ExecuteAsync(Guid userId, CancellationToken ct)
    {
        var skills = await _userSkillRepository.GetByUserIdAsync(userId, ct);
        return skills.Select(UserSkillDto.FromEntity).ToList();
    }
}
