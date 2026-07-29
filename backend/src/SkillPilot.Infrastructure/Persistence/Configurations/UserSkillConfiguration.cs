using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Infrastructure.Persistence.Configurations;

/// <summary><see cref="UserSkill"/>のEF Coreマッピング設定。</summary>
public sealed class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        builder.ToTable("UserSkills");

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

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // 同一ユーザーが同じスキル名を重複登録できないようにする(v1は完全一致のみで判定)。
        builder.HasIndex(x => new { x.UserId, x.SkillName }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
