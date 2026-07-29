using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;
using SkillPilot.Domain.Enums;

namespace SkillPilot.Application.Profile.UpdateUserSkill;

/// <summary>保有スキルの習熟レベルを更新するUseCase。</summary>
public interface IUpdateUserSkillUseCase
{
    Task<Result<UserSkillDto>> ExecuteAsync(Guid userId, Guid skillId, UpdateUserSkillRequest request, CancellationToken ct);
}

/// <summary>保有スキル更新のリクエスト。</summary>
/// <param name="Level">新しい習熟レベル。</param>
public sealed record UpdateUserSkillRequest(SkillLevel Level);

/// <inheritdoc cref="IUpdateUserSkillUseCase" />
public sealed class UpdateUserSkillUseCase : IUpdateUserSkillUseCase
{
    private readonly IUserSkillRepository _userSkillRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserSkillUseCase(IUserSkillRepository userSkillRepository, IUnitOfWork unitOfWork)
    {
        _userSkillRepository = userSkillRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<UserSkillDto>> ExecuteAsync(Guid userId, Guid skillId, UpdateUserSkillRequest request, CancellationToken ct)
    {
        var skill = await _userSkillRepository.GetByIdAsync(skillId, userId, ct);
        if (skill is null)
            return Error.NotFound("USER_SKILL_NOT_FOUND", "指定されたスキルが見つかりません。");

        skill.UpdateLevel(request.Level);
        await _unitOfWork.SaveChangesAsync(ct);

        return UserSkillDto.FromEntity(skill);
    }
}
