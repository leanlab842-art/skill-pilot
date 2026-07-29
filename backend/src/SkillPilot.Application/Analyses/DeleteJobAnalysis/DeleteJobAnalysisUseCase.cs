using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Analyses.DeleteJobAnalysis;

/// <summary>求人分析を削除(論理削除)するUseCase。</summary>
public interface IDeleteJobAnalysisUseCase
{
    Task<Result> ExecuteAsync(Guid userId, Guid analysisId, CancellationToken ct);
}

/// <inheritdoc cref="IDeleteJobAnalysisUseCase" />
public sealed class DeleteJobAnalysisUseCase : IDeleteJobAnalysisUseCase
{
    private readonly IJobAnalysisRepository _jobAnalysisRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteJobAnalysisUseCase(IJobAnalysisRepository jobAnalysisRepository, IUnitOfWork unitOfWork)
    {
        _jobAnalysisRepository = jobAnalysisRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> ExecuteAsync(Guid userId, Guid analysisId, CancellationToken ct)
    {
        var analysis = await _jobAnalysisRepository.GetByIdAsync(analysisId, userId, ct);
        if (analysis is null)
            return Error.NotFound("JOB_ANALYSIS_NOT_FOUND", "指定された求人分析が見つかりません。");

        analysis.MarkDeleted();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
