using Microsoft.Extensions.Logging;
using SkillPilot.WebApi.Common;

namespace SkillPilot.WebApi.Middleware;

/// <summary>
/// 想定外の例外(3層の例外設計における③。<c>docs/architecture.md</c>参照)を最終的に捕捉し、
/// ログ出力の上で共通のエラーレスポンス形式(500)を返す。
/// 想定内のビジネス失敗(<see cref="SkillPilot.Application.Common.Results.Result"/>の失敗)は
/// ここに到達せず、各Controllerで<see cref="ResultExtensions"/>により処理済みのため対象外。
/// </summary>
public sealed partial class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.UnhandledException(_logger, context.Request.Method, context.Request.Path, ex);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new ErrorResponse(new ErrorDetail("INTERNAL_SERVER_ERROR", "予期しないエラーが発生しました。"));
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    // LoggerMessageソースジェネレータを使うことで、ログを出力しない場合の文字列補間・ボックス化を避け、
    // 実行時コストを抑える(CA1848の推奨に従った実装)。
    private static partial class Log
    {
        [LoggerMessage(Level = LogLevel.Error, Message = "予期しない例外が発生しました。{Method} {Path}")]
        public static partial void UnhandledException(ILogger logger, string method, PathString path, Exception exception);
    }
}
