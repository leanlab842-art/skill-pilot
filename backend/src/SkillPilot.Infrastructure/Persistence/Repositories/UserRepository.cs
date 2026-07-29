using Microsoft.EntityFrameworkCore;
using SkillPilot.Application.Abstractions.Persistence;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Infrastructure.Persistence.Repositories;

/// <inheritdoc cref="IUserRepository" />
public sealed class UserRepository : IUserRepository
{
    private readonly SkillPilotDbContext _db;

    public UserRepository(SkillPilotDbContext db) => _db = db;

    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        // FindAsyncは主キー検索専用。同一UnitOfWork内で追跡中のエンティティがあれば
        // DB往復せずそれを返すため、単純なPK検索ではこちらを優先する。
        await _db.Users.FindAsync([id], ct);

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(Email email, CancellationToken ct) =>
        _db.Users.SingleOrDefaultAsync(u => u.Email == email, ct);

    /// <inheritdoc />
    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct) =>
        _db.Users.AnyAsync(u => u.Email == email, ct);

    /// <inheritdoc />
    public Task AddAsync(User user, CancellationToken ct)
    {
        // Addは変更トラッカーに登録するだけの同期処理で、DB往復は発生しない
        // (実際の保存はIUnitOfWork.SaveChangesAsyncで行う)。
        _db.Users.Add(user);
        return Task.CompletedTask;
    }
}
