namespace SkillPilot.Domain.ValueObjects;

/// <summary>スキル名。空文字・長すぎる値を防ぐ。</summary>
/// <remarks>
/// v1では表記ゆれ(例: "React"と"react")の正規化は行わず、完全一致のみで比較する。
/// 将来正規化(大文字小文字統一等)が必要になった場合も、この型の内部実装を変更するだけで済む。
/// </remarks>
public sealed record SkillName
{
    /// <summary>スキル名の文字列。</summary>
    public string Value { get; }

    private SkillName(string value) => Value = value;

    /// <summary>スキル名文字列から<see cref="SkillName"/>を生成する。</summary>
    /// <param name="value">検証対象のスキル名文字列。</param>
    /// <exception cref="ArgumentException">空文字、または100文字を超える場合。</exception>
    public static SkillName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("スキル名は必須です。", nameof(value));
        if (value.Length > 100)
            throw new ArgumentException("スキル名は100文字以内で入力してください。", nameof(value));

        return new SkillName(value.Trim());
    }

    public override string ToString() => Value;
}
