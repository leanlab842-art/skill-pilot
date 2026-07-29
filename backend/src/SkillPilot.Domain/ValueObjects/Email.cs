namespace SkillPilot.Domain.ValueObjects;

/// <summary>ログインIDとして使うメールアドレス。形式を検証し、小文字に正規化して保持する。</summary>
public sealed record Email
{
    /// <summary>正規化済みのメールアドレス文字列。</summary>
    public string Value { get; }

    private Email(string value) => Value = value;

    /// <summary>メールアドレス文字列から<see cref="Email"/>を生成する。</summary>
    /// <param name="value">検証対象のメールアドレス文字列。</param>
    /// <exception cref="ArgumentException">形式が不正な場合。</exception>
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
            throw new ArgumentException("メールアドレスの形式が不正です。", nameof(value));

        return new Email(value.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;
}
