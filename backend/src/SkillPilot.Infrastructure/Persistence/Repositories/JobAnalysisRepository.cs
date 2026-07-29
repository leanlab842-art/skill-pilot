using Microsoft.EntityFrameworkCore;
using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Domain.Entities;

namespace SkillPilot.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IJobAnalysisRepository" />
public sealed class JobAnalysisRepository : IJobAnalysisRepository
{
    private readonly SkillPilotDbContext _db;

    public JobAnalysisRepository(SkillPilotDbContext db) => _db = db;

    /// <inheritdoc />
    public Task<JobAnalysis?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct) =>
        _db.JobAnalyses
            .Include(a => a.SkillResults)
            .Include(a => a.Roadmap)
            .SingleOrDefaultAsync(a => a.Id == id && a.UserId == userId, ct);

    /// <inheritdoc />
    public async Task<(IReadOnlyList<JobAnalysis> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = _db.JobAnalyses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt);

        // 件数取得と一覧取得を分ける(EF CoreはCount+Skip/Takeを1クエリにまとめてくれないため)。
        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public Task AddAsync(JobAnalysis analysis, CancellationToken ct)
    {
        _db.JobAnalyses.Add(analysis);
        return Task.CompletedTask;
    }
}
