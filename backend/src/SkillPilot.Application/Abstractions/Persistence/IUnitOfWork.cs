namespace SkillPilot.Application.Abstractions.Persistence;

/// <summary>
/// 複数のRepositoryにまたがる変更を1つのトランザクションとして確定させる。
/// 実体はDbContextの<c>SaveChangesAsync</c>をラップしたもの。
/// </summary>
public interface IUnitOfWork
{
    /// <summary>追跡中の変更をすべて永続化する。</summary>
    Task<int> SaveChangesAsync(CancellationToken ct);
}
