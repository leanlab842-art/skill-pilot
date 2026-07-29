using Microsoft.AspNetCore.Mvc;
using SkillPilot.Application.Analyses.CompleteRoadmapItem;
using SkillPilot.WebApi.Common;

namespace SkillPilot.WebApi.Controllers;

/// <summary>求人分析に紐づく学習ロードマップを扱う。</summary>
[ApiController]
[Route("api/v1/analyses/{analysisId:guid}/roadmap")]
public sealed class LearningRoadmapController : ControllerBase
{
    private readonly ICompleteRoadmapItemUseCase _completeRoadmapItemUseCase;

    public LearningRoadmapController(ICompleteRoadmapItemUseCase completeRoadmapItemUseCase) =>
        _completeRoadmapItemUseCase = completeRoadmapItemUseCase;

    /// <summary>学習ロードマップの指定項目を完了済みにする。</summary>
    /// <remarks>
    /// v1では「完了にする」操作のみをサポートする(未完了へ戻す操作は現時点で未実装のため、
    /// リクエストボディは受け取らない)。
    /// </remarks>
    [HttpPatch("{roadmapItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid analysisId, Guid roadmapItemId, CancellationToken ct)
    {
        var result = await _completeRoadmapItemUseCase.ExecuteAsync(User.GetUserId(), analysisId, roadmapItemId, ct);
        return result.ToActionResult(this);
    }
}
