using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillPilot.Domain.Entities;
using SkillPilot.Domain.ValueObjects;

namespace SkillPilot.Infrastructure.Persistence.Configurations;

/// <summary><see cref="User"/>のEF Coreマッピング設定。</summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);
        // Idはアプリケーション側(BaseEntity)でGuid.NewGuid()により生成済みのため、
        // DBに値の生成を任せない。
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Email(ValueObject)は単一プロパティのラッパーなのでOwnsOneではなくHasConversionで
        // varchar(255)に変換する。
        builder.Property(x => x.Email)
            .HasConversion(email => email.Value, value => Email.Create(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.PasswordHash)
            .HasConversion(hash => hash.Value, value => HashedPassword.Create(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        // Skills/JobAnalysesはprivateフィールドで保持しているため、フィールド経由のアクセスを許可する。
        builder.Navigation(x => x.Skills).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.JobAnalyses).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
