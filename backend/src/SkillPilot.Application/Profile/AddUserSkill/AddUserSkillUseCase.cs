using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.Enums;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Application.Profile.AddUserSkill;

/// <summary>保有スキルを登録するUseCase。</summary>
public interface IAddUserSkillUseCase
{
    Task<Result<UserSkillDto>> ExecuteAsync(Guid userId, AddUserSkillRequest request, CancellationToken ct);
}

/// <summary>保有スキル登録のリクエスト。</summary>
/// <param name="SkillName">スキル名。</param>
/// <param name="Level">習熟レベル。</param>
public sealed record AddUserSkillRequest(string SkillName, SkillLevel Level);

/// <inheritdoc cref="IAddUserSkillUseCase" />
public sealed class AddUserSkillUseCase : IAddUserSkillUseCase
{
    private readonly IUserSkillRepository _userSkillRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddUserSkillUseCase(IUserSkillRepository userSkillRepository, IUnitOfWork unitOfWork)
    {
        _userSkillRepository = userSkillRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<UserSkillDto>> ExecuteAsync(Guid userId, AddUserSkillRequest request, CancellationToken ct)
    {
        SkillName skillName;
        try
        {
            skillName = SkillName.Create(request.SkillName);
        }
        catch (ArgumentException ex)
        {
            return Error.Validation("INVALID_SKILL_NAME", ex.Message);
        }

        if (await _userSkillRepository.ExistsByUserIdAndNameAsync(userId, skillName, ct))
            return Error.Conflict("SKILL_ALREADY_REGISTERED", "このスキルは既に登録されています。");

        var skill = new UserSkill(userId, skillName, request.Level);

        await _userSkillRepository.AddAsync(skill, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return UserSkillDto.FromEntity(skill);
    }
}
