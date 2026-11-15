using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id)
            .HasConversion(new StronglyTypedIdConverter<UserId, int>(id => id.Value, value => new UserId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<User>().Property(u => u.Name).IsRequired().HasMaxLength(160);
        builder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(254);
        builder.Entity<User>().Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
        builder.Entity<User>().Property(u => u.Role).IsRequired().HasMaxLength(40);
        builder.Entity<User>().Property(u => u.Phone).HasMaxLength(50);
        builder.Entity<User>().Property(u => u.Position).HasMaxLength(140);
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        builder.Entity<User>().HasIndex(u => new { u.CompanyId, u.Role });

        builder.Entity<PasswordResetToken>().HasKey(t => t.Id);
        builder.Entity<PasswordResetToken>().Property(t => t.Id)
            .HasConversion(new StronglyTypedIdConverter<PasswordResetTokenId, int>(id => id.Value,
                value => new PasswordResetTokenId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<PasswordResetToken>().Property(t => t.TokenHash).IsRequired().HasMaxLength(64);
        builder.Entity<PasswordResetToken>().Ignore(t => t.IsUsable);
        builder.Entity<PasswordResetToken>().HasIndex(t => t.TokenHash).IsUnique();
        builder.Entity<PasswordResetToken>().HasIndex(t => t.UserId);
    }
}
