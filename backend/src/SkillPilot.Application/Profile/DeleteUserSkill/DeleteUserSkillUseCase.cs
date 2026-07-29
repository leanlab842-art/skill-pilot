using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Profile.DeleteUserSkill;

/// <summary>保有スキルを削除するUseCase。</summary>
public interface IDeleteUserSkillUseCase
{
    Task<Result> ExecuteAsync(Guid userId, Guid skillId, CancellationToken ct);
}

/// <inheritdoc cref="IDeleteUserSkillUseCase" />
public sealed class DeleteUserSkillUseCase : IDeleteUserSkillUseCase
{
    private readonly IUserSkillRepository _userSkillRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserSkillUseCase(IUserSkillRepository userSkillRepository, IUnitOfWork unitOfWork)
    {
        _userSkillRepository = userSkillRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> ExecuteAsync(Guid userId, Guid skillId, CancellationToken ct)
    {
        var skill = await _userSkillRepository.GetByIdAsync(skillId, userId, ct);
        if (skill is null)
            return Error.NotFound("USER_SKILL_NOT_FOUND", "指定されたスキルが見つかりません。");

        _userSkillRepository.Remove(skill);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
