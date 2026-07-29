using Microsoft.EntityFrameworkCore;
using SkillPilot.Domain.Entities;

namespace SkillPilot.Infrastructure.Persistence;

/// <summary>SkillPilotのEF Core DbContext。</summary>
public sealed class SkillPilotDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserSkill> UserSkills => Set<UserSkill>();

    public DbSet<JobAnalysis> JobAnalyses => Set<JobAnalysis>();

    public DbSet<SkillResult> SkillResults => Set<SkillResult>();

    public DbSet<LearningRoadmap> LearningRoadmaps => Set<LearningRoadmap>();

    public SkillPilotDbContext(DbContextOptions<SkillPilotDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 各エンティティのマッピングはIEntityTypeConfiguration<T>としてConfigurations/配下に
        // 分離する。DbContext自体にはFluent APIの詳細を書かず、ここでは読み込みのみ行う。
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SkillPilotDbContext).Assembly);
    }
}
