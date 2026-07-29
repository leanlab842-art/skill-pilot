using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Analyses.GetJobAnalysisDetail;

/// <summary>求人分析の詳細を取得するUseCase。</summary>
public interface IGetJobAnalysisDetailUseCase
{
    Task<Result<JobAnalysisDetailDto>> ExecuteAsync(Guid userId, Guid analysisId, CancellationToken ct);
}

/// <inheritdoc cref="IGetJobAnalysisDetailUseCase" />
public sealed class GetJobAnalysisDetailUseCase : IGetJobAnalysisDetailUseCase
{
    private readonly IJobAnalysisRepository _jobAnalysisRepository;

    public GetJobAnalysisDetailUseCase(IJobAnalysisRepository jobAnalysisRepository) => _jobAnalysisRepository = jobAnalysisRepository;

    /// <inheritdoc />
    public async Task<Result<JobAnalysisDetailDto>> ExecuteAsync(Guid userId, Guid analysisId, CancellationToken ct)
    {
        var analysis = await _jobAnalysisRepository.GetByIdAsync(analysisId, userId, ct);
        if (analysis is null)
            return Error.NotFound("JOB_ANALYSIS_NOT_FOUND", "指定された求人分析が見つかりません。");

        return JobAnalysisDetailDto.FromEntity(analysis);
    }
}
