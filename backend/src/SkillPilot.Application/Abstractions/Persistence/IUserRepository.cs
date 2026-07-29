using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Application.Abstractions.Persistence;

/// <summary><see cref="User"/>の永続化を担う。</summary>
public interface IUserRepository
{
    /// <summary>Idでユーザーを取得する。見つからない場合はnull。</summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>メールアドレスでユーザーを取得する。見つからない場合はnull。</summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken ct);

    /// <summary>指定したメールアドレスが既に登録済みかどうかを判定する。</summary>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct);

    /// <summary>新規ユーザーを追加する。</summary>
    Task AddAsync(User user, CancellationToken ct);
}
