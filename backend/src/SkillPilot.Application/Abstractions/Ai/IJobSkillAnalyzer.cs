namespace SkillPilot.Application.Abstractions.Ai;

/// <summary>
/// 求人本文をAIに渡し、必要スキルと学習プランを抽出する。
/// Claude/OpenAIそれぞれの実装がこのインターフェースの背後に隠れることで、
/// Application層はどちらのAPIが使われているかを一切知らずに済む(OCP/DIP)。
/// </summary>
public interface IJobSkillAnalyzer
{
    /// <summary>求人本文を分析する。</summary>
    /// <param name="jobDescription">求人本文。</param>
    /// <param name="ct">キャンセルトークン。</param>
    /// <exception cref="AiAnalysisException">AI呼び出しに失敗した場合。</exception>
    Task<JobSkillAnalysisResult> AnalyzeAsync(string jobDescription, CancellationToken ct);
}
