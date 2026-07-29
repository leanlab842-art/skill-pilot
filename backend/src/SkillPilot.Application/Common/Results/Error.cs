using System.Diagnostics.CodeAnalysis;

namespace SkillPilot.Application.Common.Results;

/// <summary>
/// <see cref="Result"/> / <see cref="Result{T}"/>が失敗した際の種別。
/// WebApi層でHTTPステータスコードに変換される
/// (Validation→400, NotFound→404, Conflict→409)。
/// </summary>
/// <remarks>
/// Forbiddenは設けない。所有者チェックに失敗した場合も、リソースの存在を推測させないため
/// NotFoundとして扱う方針としている(<c>docs/api.md</c>で決定済み)。
/// </remarks>
public enum ErrorType
{
    /// <summary>入力値が不正、または業務ルールを満たさない。</summary>
    Validation,

    /// <summary>指定されたリソースが存在しない、または自分の所有物ではない。</summary>
    NotFound,

    /// <summary>既存のリソースと重複する、または競合する。</summary>
    Conflict
}

/// <summary>UseCaseの失敗を表す値。</summary>
/// <param name="Type">エラーの種別。</param>
/// <param name="Code">クライアントが分岐処理に使える固定コード(例: "EMAIL_ALREADY_REGISTERED")。</param>
/// <param name="Message">人が読める説明文。</param>
[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords",
    Justification = "本プロジェクトはC#専用でVB.NET等との相互運用を想定しないため、ドメインとして最も明確な名前を優先する。")]
public sealed record Error(ErrorType Type, string Code, string Message)
{
    /// <summary>バリデーションエラーを生成する。</summary>
    public static Error Validation(string code, string message) => new(ErrorType.Validation, code, message);

    /// <summary>リソース未検出エラーを生成する。</summary>
    public static Error NotFound(string code, string message) => new(ErrorType.NotFound, code, message);

    /// <summary>重複・競合エラーを生成する。</summary>
    public static Error Conflict(string code, string message) => new(ErrorType.Conflict, code, message);
}
