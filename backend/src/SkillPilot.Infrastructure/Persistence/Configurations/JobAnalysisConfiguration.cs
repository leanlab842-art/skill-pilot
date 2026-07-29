using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Infrastructure.Persistence.Configurations;

/// <summary><see cref="JobAnalysis"/>のEF Coreマッピング設定。</summary>
public sealed class JobAnalysisConfiguration : IEntityTypeConfiguration<JobAnalysis>
{
    public void Configure(EntityTypeBuilder<JobAnalysis> builder)
    {
        builder.ToTable("JobAnalyses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.JobTitle).HasMaxLength(200).IsRequired();
        builder.Property(x => x.JobUrl).HasMaxLength(2048);
        builder.Property(x => x.JobDescription).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // MatchRateはnullable ValueObject。null/非nullの両方をVOのCreateに通す変換にする。
        builder.Property(x => x.MatchRate)
            .HasConversion(
                rate => rate == null ? (int?)null : rate.Value,
                value => value == null ? null : MatchRate.Create(value.Value));

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // 一覧取得(ユーザーIdで絞り込み、論理削除済みを除く)を高速化するための複合インデックス。
        builder.HasIndex(x => new { x.UserId, x.DeletedAt });

        // 論理削除されたレコードをすべてのクエリから自動的に除外する。
        // Domain層は「削除済みを除外する」判断を持たず、この関心事はInfrastructure層に閉じる。
        builder.HasQueryFilter(x => x.DeletedAt == null);

        builder.HasOne(x => x.User)
            .WithMany(x => x.JobAnalyses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.SkillResults)
            .WithOne(x => x.Analysis)
            .HasForeignKey(x => x.AnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Roadmap)
            .WithOne(x => x.Analysis)
            .HasForeignKey(x => x.AnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.SkillResults).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Roadmap).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
