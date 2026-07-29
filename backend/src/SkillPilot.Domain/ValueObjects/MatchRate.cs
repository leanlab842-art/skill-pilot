namespace SkillPilot.Domain.ValueObjects;

/// <summary>必須スキルの充足率(0〜100)。</summary>
public sealed record MatchRate
{
    /// <summary>0〜100の整数値。</summary>
    public int Value { get; }

    private MatchRate(int value) => Value = value;

    /// <summary>算出済みの数値から<see cref="MatchRate"/>を生成する。</summary>
    /// <param name="value">0〜100の範囲の数値。</param>
    /// <exception cref="ArgumentOutOfRangeException">0〜100の範囲外の場合。</exception>
    public static MatchRate Create(int value)
    {
        if (value is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "MatchRateは0〜100の範囲で指定してください。");

        return new MatchRate(value);
    }
}
