using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillPilot.Domain.Entities;

namespace SkillPilot.Infrastructure.Persistence.Configurations;

/// <summary><see cref="LearningRoadmap"/>のEF Coreマッピング設定。</summary>
public sealed class LearningRoadmapConfiguration : IEntityTypeConfiguration<LearningRoadmap>
{
    public void Configure(EntityTypeBuilder<LearningRoadmap> builder)
    {
        builder.ToTable("LearningRoadmaps");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description);

        builder.Property(x => x.Week).IsRequired();
        builder.ToTable(t => t.HasCheckConstraint("CK_LearningRoadmaps_Week", "\"Week\" >= 1"));

        builder.Property(x => x.Completed).IsRequired();
        builder.Property(x => x.CompletedAt);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // JobAnalysis側にDeletedAtのクエリフィルタがあるため、こちら側にも同じ条件を設定する
        // (SkillResultConfigurationと同じ理由。EF Coreの警告に従った対応)。
        builder.HasQueryFilter(x => x.Analysis.DeletedAt == null);

        // JobAnalysisとの関係(1:多)はJobAnalysisConfiguration側で設定済み。
        // SkillResultとの関係(多:1、任意)はここで設定する。SkillResult側に逆ナビゲーションを
        // 持たせていないため WithMany() は無名にする。SkillResultが削除されても
        // LearningRoadmap自体は残し、対応関係だけを外す(ON DELETE SET NULL)。
        builder.HasOne(x => x.SkillResult)
            .WithMany()
            .HasForeignKey(x => x.SkillResultId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
