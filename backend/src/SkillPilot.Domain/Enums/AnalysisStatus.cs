namespace SkillPilot.Domain.Enums;

/// <summary>求人分析(AI呼び出し)の進行状態。</summary>
public enum AnalysisStatus
{
    /// <summary>未分析、または再分析待ち。</summary>
    Pending,

    /// <summary>分析完了。</summary>
    Completed,

    /// <summary>分析失敗(AI呼び出しエラー等)。</summary>
    Failed
}
