using SkillPilot.Domain.Entities;

namespace SkillPilot.Application.Abstractions.Persistence;

/// <summary><see cref="JobAnalysis"/>集約(JobAnalysis + SkillResult + LearningRoadmap)の永続化を担う。</summary>
public interface IJobAnalysisRepository
{
    /// <summary>
    /// Idと所有者Idで求人分析を取得する(SkillResult/Roadmapを含む)。
    /// 見つからない、または他人の所有物の場合はnull。
    /// </summary>
    Task<JobAnalysis?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct);

    /// <summary>指定したユーザーの求人分析一覧をページングして取得する(論理削除済みは含まない)。</summary>
    /// <returns>該当ページの一覧と、全体の件数。</returns>
    Task<(IReadOnlyList<JobAnalysis> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken ct);

    /// <summary>新規に求人分析を追加する。</summary>
    Task AddAsync(JobAnalysis analysis, CancellationToken ct);
}
