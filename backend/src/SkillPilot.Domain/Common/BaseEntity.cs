namespace SkillPilot.Domain.Common;

/// <summary>
/// 全エンティティ共通のId/CreatedAt/UpdatedAtを集約する基底クラス。
/// EF Core等の外部ライブラリには一切依存しない(Pure Domain)。
/// </summary>
public abstract class BaseEntity : IEquatable<BaseEntity>
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; protected set; } = DateTimeOffset.UtcNow;

    // 状態を変更したサブクラスのメソッドから呼び出し、更新日時を進める
    protected void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    // エンティティはIdが同じであれば同一とみなす(値の一致で比較するValueObjectとは異なる)
    public bool Equals(BaseEntity? other) =>
        other is not null && GetType() == other.GetType() && Id == other.Id;

    public override bool Equals(object? obj) => Equals(obj as BaseEntity);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
