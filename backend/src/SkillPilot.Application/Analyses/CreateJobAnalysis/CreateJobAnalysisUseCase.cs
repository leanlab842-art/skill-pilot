using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Application.Common.Dtos;
using SkillPilot.Application.Common.Results;
using SkillPilot.Domain.Entities;

namespace SkillPilot.Application.Analyses.CreateJobAnalysis;

/// <summary>求人を登録し、AI分析を実行するUseCase。</summary>
public interface ICreateJobAnalysisUseCase
{
    Task<Result<JobAnalysisDetailDto>> ExecuteAsync(Guid userId, CreateJobAnalysisRequest request, CancellationToken ct);
}

/// <summary>求人登録のリクエスト。</summary>
/// <param name="CompanyName">会社名。</param>
/// <param name="JobTitle">求人タイトル。</param>
/// <param name="JobUrl">求人ページの参照用URL(任意)。</param>
/// <param name="JobDescription">求人本文。</param>
public sealed record CreateJobAnalysisRequest(string CompanyName, string JobTitle, string? JobUrl, string JobDescription);

/// <inheritdoc cref="ICreateJobAnalysisUseCase" />
public sealed class CreateJobAnalysisUseCase : ICreateJobAnalysisUseCase
{
    private readonly IJobAnalysisRepository _jobAnalysisRepository;
    private readonly JobAnalysisRunner _runner;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJobAnalysisUseCase(IJobAnalysisRepository jobAnalysisRepository, JobAnalysisRunner runner, IUnitOfWork unitOfWork)
    {
        _jobAnalysisRepository = jobAnalysisRepository;
        _runner = runner;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<JobAnalysisDetailDto>> ExecuteAsync(Guid userId, CreateJobAnalysisRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.JobDescription))
            return Error.Validation("JOB_DESCRIPTION_REQUIRED", "求人本文は必須です。");

        var analysis = new JobAnalysis(userId, request.CompanyName, request.JobTitle, request.JobUrl, request.JobDescription);

        await _runner.RunAsync(analysis, userId, request.JobDescription, ct);

        await _jobAnalysisRepository.AddAsync(analysis, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return JobAnalysisDetailDto.FromEntity(analysis);
    }
}
