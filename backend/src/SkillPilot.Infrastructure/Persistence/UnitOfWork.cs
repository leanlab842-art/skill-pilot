using SkillPilot.Application.Abstractions.Persistence;

namespace SkillPilot.Infrastructure.Persistence;

/// <inheritdoc cref="IUnitOfWork" />
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SkillPilotDbContext _db;

    public UnitOfWork(SkillPilotDbContext db) => _db = db;

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
