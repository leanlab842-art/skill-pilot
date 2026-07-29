namespace SkillPilot.Domain.Common;

/// <summary>
/// 論理削除に対応するエンティティが実装するインターフェース。
/// 全エンティティに一律で<c>DeletedAt</c>を持たせるのではなく、必要なものだけに実装させる
/// (インターフェース分離の原則)。将来他のエンティティにも論理削除が必要になった場合は、
/// このインターフェースを実装するだけで拡張できる。
/// </summary>
public interface ISoftDeletable
{
    /// <summary>削除日時。nullの場合は未削除。</summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>削除済みかどうか。</summary>
    bool IsDeleted => DeletedAt is not null;

    /// <summary>論理削除としてマークする。</summary>
    void MarkDeleted();
}
