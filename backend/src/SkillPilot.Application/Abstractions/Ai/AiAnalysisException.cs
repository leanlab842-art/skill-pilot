namespace SkillPilot.Application.Abstractions.Ai;

/// <summary>
/// AI(Claude/OpenAI等)による求人分析の呼び出しに失敗したことを表す例外。
/// 通信エラー・タイムアウト・レスポンスのパース失敗など、原因を問わずこの型にラップしてスローする。
/// </summary>
/// <remarks>
/// 「想定内の外部要因による失敗」としてUseCase側が意図的にcatchし、
/// <c>JobAnalysis.FailAnalysis()</c>のようにDomainの状態遷移に変換する
/// (3層の例外設計における②。詳細は<c>docs/architecture.md</c>を参照)。
/// </remarks>
public sealed class AiAnalysisException : Exception
{
    public AiAnalysisException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
