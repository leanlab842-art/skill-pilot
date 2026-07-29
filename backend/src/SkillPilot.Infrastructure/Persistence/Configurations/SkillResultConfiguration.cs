using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Infrastructure.Persistence.Configurations;

/// <summary><see cref="SkillResult"/>のEF Coreマッピング設定。</summary>
public sealed class SkillResultConfiguration : IEntityTypeConfiguration<SkillResult>
{
    public void Configure(EntityTypeBuilder<SkillResult> builder)
    {
        builder.ToTable("SkillResults");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.SkillName)
            .HasConversion(name => name.Value, value => SkillName.Create(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Level)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.IsMissing).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        // JobAnalysis側にDeletedAtのクエリフィルタがあるため、こちら側にも同じ条件を設定する。
        // (親JobAnalysisを経由せずSkillResultを直接クエリした場合でも、削除済みの求人分析に
        // 紐づくデータが漏れて返らないようにする。EF Coreの警告に従った対応)
        builder.HasQueryFilter(x => x.Analysis.DeletedAt == null);

        // JobAnalysisとの関係はJobAnalysisConfiguration側(HasMany)で設定済みのため、ここでは行わない。
    }
}
