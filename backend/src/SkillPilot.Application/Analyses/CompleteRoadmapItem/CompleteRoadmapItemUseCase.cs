using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Results;

namespace SkillPilot.Application.Analyses.CompleteRoadmapItem;

/// <summary>学習ロードマップの項目を完了済みにするUseCase。</summary>
public interface ICompleteRoadmapItemUseCase
{
    Task<Result> ExecuteAsync(Guid userId, Guid analysisId, Guid roadmapItemId, CancellationToken ct);
}

/// <inheritdoc cref="ICompleteRoadmapItemUseCase" />
public sealed class CompleteRoadmapItemUseCase : ICompleteRoadmapItemUseCase
{
    private readonly IJobAnalysisRepository _jobAnalysisRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteRoadmapItemUseCase(IJobAnalysisRepository jobAnalysisRepository, IUnitOfWork unitOfWork)
    {
        _jobAnalysisRepository = jobAnalysisRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> ExecuteAsync(Guid userId, Guid analysisId, Guid roadmapItemId, CancellationToken ct)
    {
        var analysis = await _jobAnalysisRepository.GetByIdAsync(analysisId, userId, ct);
        if (analysis is null)
            return Error.NotFound("JOB_ANALYSIS_NOT_FOUND", "指定された求人分析が見つかりません。");

        try
        {
            analysis.CompleteRoadmapItem(roadmapItemId);
        }
        catch (InvalidOperationException)
        {
            return Error.NotFound("ROADMAP_ITEM_NOT_FOUND", "指定された学習ロードマップ項目が見つかりません。");
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
