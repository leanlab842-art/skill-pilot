using Microsoft.AspNetCore.Mvc;
using SkillPilot.Application.Analyses.CreateJobAnalysis;
using SkillPilot.Application.Analyses.DeleteJobAnalysis;
using SkillPilot.Application.Analyses.GetJobAnalyses;
using SkillPilot.Application.Analyses.GetJobAnalysisDetail;
using SkillPilot.Application.Analyses.UpdateJobAnalysis;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.WebApi.Common;

namespace SkillPilot.WebApi.Controllers;

/// <summary>求人分析(求人の登録・AI分析結果)を扱う。</summary>
[ApiController]
[Route("api/v1/analyses")]
public sealed class JobAnalysisController : ControllerBase
{
    private const int MaxPageSize = 100;

    private readonly IGetJobAnalysesUseCase _getJobAnalysesUseCase;
    private readonly ICreateJobAnalysisUseCase _createJobAnalysisUseCase;
    private readonly IGetJobAnalysisDetailUseCase _getJobAnalysisDetailUseCase;
    private readonly IUpdateJobAnalysisUseCase _updateJobAnalysisUseCase;
    private readonly IDeleteJobAnalysisUseCase _deleteJobAnalysisUseCase;

    public JobAnalysisController(
        IGetJobAnalysesUseCase getJobAnalysesUseCase,
        ICreateJobAnalysisUseCase createJobAnalysisUseCase,
        IGetJobAnalysisDetailUseCase getJobAnalysisDetailUseCase,
        IUpdateJobAnalysisUseCase updateJobAnalysisUseCase,
        IDeleteJobAnalysisUseCase deleteJobAnalysisUseCase)
    {
        _getJobAnalysesUseCase = getJobAnalysesUseCase;
        _createJobAnalysisUseCase = createJobAnalysisUseCase;
        _getJobAnalysisDetailUseCase = getJobAnalysisDetailUseCase;
        _updateJobAnalysisUseCase = updateJobAnalysisUseCase;
        _deleteJobAnalysisUseCase = deleteJobAnalysisUseCase;
    }

    /// <summary>自分の求人分析一覧をページングして取得する。</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<JobAnalysisSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var result = await _getJobAnalysesUseCase.ExecuteAsync(User.GetUserId(), new GetJobAnalysesRequest(page, pageSize), ct);
        return result.ToActionResult(this);
    }

    /// <summary>求人を登録し、AI分析を実行する(同期処理)。</summary>
    [HttpPost]
    [ProducesResponseType(typeof(JobAnalysisDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateJobAnalysisRequest request, CancellationToken ct)
    {
        var result = await _createJobAnalysisUseCase.ExecuteAsync(User.GetUserId(), request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>求人分析の詳細(必要スキル・学習ロードマップを含む)を取得する。</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobAnalysisDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct)
    {
        var result = await _getJobAnalysisDetailUseCase.ExecuteAsync(User.GetUserId(), id, ct);
        return result.ToActionResult(this);
    }

    /// <summary>求人情報を編集する。求人本文を変更した場合は自動的に再分析する。</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(JobAnalysisDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJobAnalysisRequest request, CancellationToken ct)
    {
        var result = await _updateJobAnalysisUseCase.ExecuteAsync(User.GetUserId(), id, request, ct);
        return result.ToActionResult(this);
    }

    /// <summary>求人分析を削除する(論理削除)。</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _deleteJobAnalysisUseCase.ExecuteAsync(User.GetUserId(), id, ct);
        return result.ToActionResult(this);
    }
}
