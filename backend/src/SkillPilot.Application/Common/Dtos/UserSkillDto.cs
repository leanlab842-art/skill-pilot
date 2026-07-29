using SkillPilot.Domain.Entities;
using SkillPilot.Domain.Enums;

namespace SkillPilot.Application.Common.Dtos;

/// <summary>ユーザーの保有スキル。</summary>
/// <param name="Id">保有スキルId。</param>
/// <param name="SkillName">スキル名。</param>
/// <param name="Level">習熟レベル。</param>
public sealed record UserSkillDto(Guid Id, string SkillName, SkillLevel Level)
{
    /// <summary>エンティティからDTOへ変換する。</summary>
    public static UserSkillDto FromEntity(UserSkill entity) => new(entity.Id, entity.SkillName.Value, entity.Level);
}
