using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Analyses.GetJobAnalyses;

/// <summary>求人分析の一覧を取得するUseCase。</summary>
public interface IGetJobAnalysesUseCase
{
    Task<Result<PagedResult<JobAnalysisSummaryDto>>> ExecuteAsync(Guid userId, GetJobAnalysesRequest request, CancellationToken ct);
}

/// <summary>求人分析一覧取得のリクエスト。</summary>
/// <param name="Page">ページ番号(1始まり)。</param>
/// <param name="PageSize">1ページあたりの件数。</param>
public sealed record GetJobAnalysesRequest(int Page, int PageSize);

/// <inheritdoc cref="IGetJobAnalysesUseCase" />
public sealed class GetJobAnalysesUseCase : IGetJobAnalysesUseCase
{
    private readonly IJobAnalysisRepository _jobAnalysisRepository;

    public GetJobAnalysesUseCase(IJobAnalysisRepository jobAnalysisRepository) => _jobAnalysisRepository = jobAnalysisRepository;

    /// <inheritdoc />
    public async Task<Result<PagedResult<JobAnalysisSummaryDto>>> ExecuteAsync(Guid userId, GetJobAnalysesRequest request, CancellationToken ct)
    {
        var (items, totalCount) = await _jobAnalysisRepository.GetPagedByUserIdAsync(userId, request.Page, request.PageSize, ct);

        return new PagedResult<JobAnalysisSummaryDto>(
            items.Select(JobAnalysisSummaryDto.FromEntity).ToList(),
            request.Page,
            request.PageSize,
            totalCount);
    }
}
