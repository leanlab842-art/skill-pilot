namespace SkillPilot.Domain.ValueObjects;

/// <summary>
/// ハッシュ化済みパスワード。生のパスワード文字列(<see cref="string"/>)と型レベルで区別することで、
/// 未ハッシュの値を誤って保存・比較・ログ出力してしまう事故を防ぐ。
/// </summary>
/// <remarks>
/// ハッシュ化アルゴリズム自体はInfrastructure層の<c>IPasswordHasher</c>の責務であり、
/// Domainはアルゴリズムを知らない。この型は「ハッシュ化済みの値である」という事実だけを表現する。
/// </remarks>
public sealed record HashedPassword
{
    /// <summary>ハッシュ化済みの文字列。</summary>
    public string Value { get; }

    private HashedPassword(string value) => Value = value;

    /// <summary>ハッシュ化済み文字列から<see cref="HashedPassword"/>を生成する。</summary>
    /// <param name="hashedValue">ハッシュ化済みの文字列。</param>
    /// <exception cref="ArgumentException">空文字の場合。</exception>
    public static HashedPassword Create(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new ArgumentException("ハッシュ化されたパスワードは必須です。", nameof(hashedValue));

        return new HashedPassword(hashedValue);
    }

    // ログや例外メッセージに誤ってハッシュ値そのものが出力されることを防ぐ
    public override string ToString() => "[REDACTED]";
}
