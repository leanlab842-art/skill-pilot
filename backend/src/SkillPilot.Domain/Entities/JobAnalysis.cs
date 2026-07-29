using SkillPilot.Domain.Common;
using SkillPilot.Domain.Enums;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Domain.Entities;

/// <summary>
/// ユーザーが登録した求人と、そのAI分析結果を保持する集約ルート。
/// <see cref="SkillResult"/>と<see cref="LearningRoadmap"/>は、このエンティティのメソッドを
/// 通してのみ生成・更新される。
/// </summary>
public sealed class JobAnalysis : BaseEntity, ISoftDeletable
{
    /// <summary>所有者のユーザーId。</summary>
    public Guid UserId { get; private set; }

    /// <summary>所有者への参照ナビゲーション。</summary>
    public User User { get; private set; } = null!;

    /// <summary>応募先の会社名。</summary>
    public string CompanyName { get; private set; } = null!;

    /// <summary>求人タイトル。</summary>
    public string JobTitle { get; private set; } = null!;

    /// <summary>求人ページの参照用URL(任意)。サーバーはこのURLに直接アクセスしない。</summary>
    public string? JobUrl { get; private set; }

    /// <summary>ユーザーが貼り付けた求人本文。</summary>
    public string JobDescription { get; private set; } = null!;

    /// <summary>AI分析の状態。</summary>
    public AnalysisStatus Status { get; private set; } = AnalysisStatus.Pending;

    /// <summary>必須スキルの充足率。分析が完了するまではnull。</summary>
    public MatchRate? MatchRate { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted => DeletedAt is not null;

    private readonly List<SkillResult> _skillResults = [];

    /// <summary>AIが抽出した必要スキルと不足判定結果の一覧。</summary>
    public IReadOnlyCollection<SkillResult> SkillResults => _skillResults.AsReadOnly();

    private readonly List<LearningRoadmap> _roadmap = [];

    /// <summary>AIが生成した週単位の学習プラン。</summary>
    public IReadOnlyCollection<LearningRoadmap> Roadmap => _roadmap.AsReadOnly();

    private JobAnalysis()
    {
    }

    /// <summary>求人分析を新規作成する。作成直後は<see cref="AnalysisStatus.Pending"/>となる。</summary>
    /// <param name="userId">所有者のユーザーId。</param>
    /// <param name="companyName">応募先の会社名。</param>
    /// <param name="jobTitle">求人タイトル。</param>
    /// <param name="jobUrl">求人ページの参照用URL(任意)。</param>
    /// <param name="jobDescription">求人本文。</param>
    public JobAnalysis(Guid userId, string companyName, string jobTitle, string? jobUrl, string jobDescription)
    {
        UserId = userId;
        CompanyName = companyName;
        JobTitle = jobTitle;
        JobUrl = jobUrl;
        JobDescription = jobDescription;
    }

    /// <summary>
    /// AI分析が成功した際に呼び出す。必要スキル・学習プラン・マッチ率を一括で確定させる。
    /// </summary>
    /// <param name="skillResults">算出済みの必要スキル・不足判定結果。</param>
    /// <param name="roadmap">生成された学習プラン。</param>
    /// <param name="matchRate">算出済みのマッチ率。</param>
    public void CompleteAnalysis(IEnumerable<SkillResult> skillResults, IEnumerable<LearningRoadmap> roadmap, MatchRate matchRate)
    {
        _skillResults.Clear();
        _skillResults.AddRange(skillResults);
        _roadmap.Clear();
        _roadmap.AddRange(roadmap);
        MatchRate = matchRate;
        Status = AnalysisStatus.Completed;
        Touch();
    }

    /// <summary>AI分析の呼び出しに失敗した際に呼び出す。</summary>
    public void FailAnalysis()
    {
        Status = AnalysisStatus.Failed;
        Touch();
    }

    /// <summary>
    /// 求人情報を編集する。求人本文を変更した場合は再分析が必要になるため、
    /// Statusを再度<see cref="AnalysisStatus.Pending"/>へ戻す。
    /// </summary>
    /// <param name="companyName">応募先の会社名。</param>
    /// <param name="jobTitle">求人タイトル。</param>
    /// <param name="jobUrl">求人ページの参照用URL(任意)。</param>
    /// <param name="jobDescription">求人本文。</param>
    public void UpdateJobPosting(string companyName, string jobTitle, string? jobUrl, string jobDescription)
    {
        var descriptionChanged = JobDescription != jobDescription;

        CompanyName = companyName;
        JobTitle = jobTitle;
        JobUrl = jobUrl;
        JobDescription = jobDescription;

        // 本文が実際に変わった場合のみ再分析が必要になるため、その場合だけPendingへ戻す。
        // 会社名など他の項目だけの編集で、既存のSkillResults/Roadmapを無意味に無効化しない。
        if (descriptionChanged)
            Status = AnalysisStatus.Pending;

        Touch();
    }

    /// <summary>学習ロードマップの指定項目を完了済みにする。</summary>
    /// <param name="roadmapItemId">完了させる学習ロードマップ項目のId。</param>
    /// <exception cref="InvalidOperationException">指定したIdの項目がこの分析に存在しない場合。</exception>
    public void CompleteRoadmapItem(Guid roadmapItemId)
    {
        var item = _roadmap.SingleOrDefault(r => r.Id == roadmapItemId)
            ?? throw new InvalidOperationException("指定されたロードマップ項目がこの分析に存在しません。");

        item.MarkCompleted();
        Touch();
    }

    /// <inheritdoc />
    public void MarkDeleted() => DeletedAt = DateTimeOffset.UtcNow;
}
