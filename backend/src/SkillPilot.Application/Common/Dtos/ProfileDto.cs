using SkillPilot.Domain.Entities;

namespace SkillPilot.Application.Common.Dtos;

/// <summary>ユーザーのプロフィール情報。</summary>
/// <param name="Id">ユーザーId。</param>
/// <param name="Name">表示名。</param>
/// <param name="Email">メールアドレス。</param>
public sealed record ProfileDto(Guid Id, string Name, string Email)
{
    /// <summary>エンティティからDTOへ変換する。</summary>
    public static ProfileDto FromEntity(User entity) => new(entity.Id, entity.Name, entity.Email.Value);
}
