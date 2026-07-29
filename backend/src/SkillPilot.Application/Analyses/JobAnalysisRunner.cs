using SkillPilot.Application.Abstractions.Ai;
using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.Services;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Application.Analyses;

/// <summary>
/// 求人本文に対してAI分析を実行し、<see cref="JobAnalysis"/>へ結果を反映する処理。
/// </summary>
/// <remarks>
/// <see cref="CreateJobAnalysis.CreateJobAnalysisUseCase"/>と
/// <see cref="UpdateJobAnalysis.UpdateJobAnalysisUseCase"/>の両方から利用される共通ロジックのため、
/// どちらのUseCaseにも属させず専用のクラスとして切り出している(重複回避)。
/// UseCaseのpublicコンストラクタの引数として使うため、このクラス自体もpublicにする必要がある
/// (C#の仕様上、publicメンバーの引数型はそれ以上に可視性を狭められない)。
/// UseCaseインターフェース(<c>ICreateJobAnalysisUseCase</c>等)としては公開されないため、
/// Application層の外部から直接使われることは想定していない。
/// </remarks>
public sealed class JobAnalysisRunner
{
    private readonly IUserSkillRepository _userSkillRepository;
    private readonly IJobSkillAnalyzer _jobSkillAnalyzer;

    public JobAnalysisRunner(IUserSkillRepository userSkillRepository, IJobSkillAnalyzer jobSkillAnalyzer)
    {
        _userSkillRepository = userSkillRepository;
        _jobSkillAnalyzer = jobSkillAnalyzer;
    }

    /// <summary>
    /// AI呼び出し〜不足スキル算出〜分析結果の確定までを行う。
    /// AI呼び出しが失敗した場合は<see cref="JobAnalysis.FailAnalysis"/>に倒し、例外は投げない
    /// (呼び出し元のUseCaseは常にこのメソッドの完了後、Statusで結果を判定すればよい)。
    /// </summary>
    public async Task RunAsync(JobAnalysis analysis, Guid userId, string jobDescription, CancellationToken ct)
    {
        try
        {
            var aiResult = await _jobSkillAnalyzer.AnalyzeAsync(jobDescription, ct);
            var userSkills = await _userSkillRepository.GetByUserIdAsync(userId, ct);

            var requiredSkills = aiResult.Skills
                .Select(s => new RequiredSkillInput(SkillName.Create(s.Name), s.Level, s.Category))
                .ToList();

            var (skillResults, matchRate) = SkillGapCalculator.Calculate(requiredSkills, userSkills);

            var roadmap = aiResult.Roadmap
                .Select(r => new LearningRoadmap(FindSkillResultId(r.RelatedSkillName, skillResults), r.Title, r.Description, r.Week))
                .ToList();

            analysis.CompleteAnalysis(skillResults, roadmap, matchRate);
        }
        catch (AiAnalysisException)
        {
            // AI呼び出しの失敗は「想定内」。分析自体はStatus=Failedとして保存し、
            // 呼び出し元のUseCaseとしてはResultの成功を返す
            // (HTTPは200、クライアントはstatusフィールドで判定する)。
            analysis.FailAnalysis();
        }
    }

    private static Guid? FindSkillResultId(string? relatedSkillName, IReadOnlyList<SkillResult> skillResults)
    {
        if (relatedSkillName is null)
            return null;

        return skillResults
            .FirstOrDefault(s => string.Equals(s.SkillName.Value, relatedSkillName, StringComparison.OrdinalIgnoreCase))
            ?.Id;
    }
}
