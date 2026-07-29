namespace SkillPilot.Domain.Enums;

/// <summary>
/// スキルの習熟レベル。<see cref="Entities.UserSkill"/>(ユーザーの保有レベル)と
/// <see cref="Entities.SkillResult"/>(求人が求めるレベル)の双方で共用する。
/// </summary>
public enum SkillLevel
{
    /// <summary>初級。</summary>
    Beginner,

    /// <summary>中級。</summary>
    Intermediate,

    /// <summary>上級。</summary>
    Advanced
}
