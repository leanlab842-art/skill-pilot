namespace SkillPilot.WebApi.Common;

/// <summary>APIのエラーレスポンス共通形式(<c>docs/api.md</c>で定義した形式)。</summary>
/// <param name="Error">エラーの詳細。</param>
public sealed record ErrorResponse(ErrorDetail Error);

/// <summary>エラーの詳細。</summary>
/// <param name="Code">クライアントが分岐処理に使える固定コード。</param>
/// <param name="Message">人が読める説明文。</param>
public sealed record ErrorDetail(string Code, string Message);
