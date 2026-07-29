using SkillPilot.Domain.Common;

namespace SkillPilot.Domain.Entities;

/// <summary>AIが生成した週単位の学習プラン項目。</summary>
public sealed class LearningRoadmap : BaseEntity
{
    /// <summary>紐づく求人分析のId。</summary>
    public Guid AnalysisId { get; private set; }

    /// <summary>紐づく求人分析への参照ナビゲーション。</summary>
    public JobAnalysis Analysis { get; private set; } = null!;

    /// <summary>対応する不足スキルのId(あれば)。</summary>
    public Guid? SkillResultId { get; private set; }

    /// <summary>対応する不足スキルへの参照ナビゲーション。</summary>
    public SkillResult? SkillResult { get; private set; }

    /// <summary>学習項目のタイトル。</summary>
    public string Title { get; private set; } = null!;

    /// <summary>補足説明。</summary>
    public string? Description { get; private set; }

    /// <summary>何週目の学習項目か(1始まり)。</summary>
    public int Week { get; private set; }

    /// <summary>完了済みかどうか。</summary>
    public bool Completed { get; private set; }

    /// <summary>完了した日時。未完了の場合はnull。</summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    private LearningRoadmap()
    {
    }

    /// <summary>学習プラン項目を新規作成する。</summary>
    /// <param name="skillResultId">対応する不足スキルのId(あれば)。</param>
    /// <param name="title">学習項目のタイトル。</param>
    /// <param name="description">補足説明。</param>
    /// <param name="week">何週目の学習項目か(1始まり)。</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="week"/>が1未満の場合。</exception>
    public LearningRoadmap(Guid? skillResultId, string title, string? description, int week)
    {
        if (week < 1)
            throw new ArgumentOutOfRangeException(nameof(week), "Weekは1以上で指定してください。");

        SkillResultId = skillResultId;
        Title = title;
        Description = description;
        Week = week;
    }

    // JobAnalysis.CompleteRoadmapItem経由でのみ呼ばれる想定のため internal とする。
    internal void MarkCompleted()
    {
        Completed = true;
        CompletedAt = DateTimeOffset.UtcNow;
        Touch();
    }
}
