using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Analyses.UpdateJobAnalysis;

/// <summary>求人を編集するUseCase。求人本文を変更した場合は自動的に再分析する。</summary>
public interface IUpdateJobAnalysisUseCase
{
    Task<Result<JobAnalysisDetailDto>> ExecuteAsync(Guid userId, Guid analysisId, UpdateJobAnalysisRequest request, CancellationToken ct);
}

/// <summary>求人編集のリクエスト。</summary>
/// <param name="CompanyName">会社名。</param>
/// <param name="JobTitle">求人タイトル。</param>
/// <param name="JobUrl">求人ページの参照用URL(任意)。</param>
/// <param name="JobDescription">求人本文。</param>
public sealed record UpdateJobAnalysisRequest(string CompanyName, string JobTitle, string? JobUrl, string JobDescription);

/// <inheritdoc cref="IUpdateJobAnalysisUseCase" />
public sealed class UpdateJobAnalysisUseCase : IUpdateJobAnalysisUseCase
{
    private readonly IJobAnalysisRepository _jobAnalysisRepository;
    private readonly JobAnalysisRunner _runner;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateJobAnalysisUseCase(IJobAnalysisRepository jobAnalysisRepository, JobAnalysisRunner runner, IUnitOfWork unitOfWork)
    {
        _jobAnalysisRepository = jobAnalysisRepository;
        _runner = runner;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<JobAnalysisDetailDto>> ExecuteAsync(Guid userId, Guid analysisId, UpdateJobAnalysisRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.JobDescription))
            return Error.Validation("JOB_DESCRIPTION_REQUIRED", "求人本文は必須です。");

        var analysis = await _jobAnalysisRepository.GetByIdAsync(analysisId, userId, ct);
        if (analysis is null)
            return Error.NotFound("JOB_ANALYSIS_NOT_FOUND", "指定された求人分析が見つかりません。");

        // 求人本文が変わっていない場合はAI呼び出し自体を省略し、コストを抑える。
        var descriptionChanged = analysis.JobDescription != request.JobDescription;

        analysis.UpdateJobPosting(request.CompanyName, request.JobTitle, request.JobUrl, request.JobDescription);

        if (descriptionChanged)
            await _runner.RunAsync(analysis, userId, request.JobDescription, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return JobAnalysisDetailDto.FromEntity(analysis);
    }
}
