using Microsoft.AspNetCore.Mvc;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.WebApi.Common;

/// <summary>
/// UseCaseが返す<see cref="Result{T}"/>/<see cref="Result"/>をHTTPレスポンスへ変換する。
/// Controllerはこの拡張メソッドを通すだけで、Result→HTTPステータスの変換ロジックを
/// 毎回書かずに済む(Validation→400, NotFound→404, Conflict→409)。
/// </summary>
public static class ResultExtensions
{
    /// <summary>戻り値ありのUseCase結果を、成功時200/失敗時エラーレスポンスへ変換する。</summary>
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller) =>
        result.IsSuccess ? controller.Ok(result.Value) : MapError(controller, result.Error!);

    /// <summary>戻り値なしのUseCase結果を、成功時204/失敗時エラーレスポンスへ変換する。</summary>
    public static IActionResult ToActionResult(this Result result, ControllerBase controller) =>
        result.IsSuccess ? controller.NoContent() : MapError(controller, result.Error!);

    private static ObjectResult MapError(ControllerBase controller, Error error)
    {
        var response = new ErrorResponse(new ErrorDetail(error.Code, error.Message));

        return error.Type switch
        {
            ErrorType.Validation => controller.BadRequest(response),
            ErrorType.NotFound => controller.NotFound(response),
            ErrorType.Conflict => controller.Conflict(response),
            _ => controller.StatusCode(StatusCodes.Status500InternalServerError, response),
        };
    }
}
