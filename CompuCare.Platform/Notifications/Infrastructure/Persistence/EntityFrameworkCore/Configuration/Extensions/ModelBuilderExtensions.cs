using CompuCare.Platform.Notifications.Domain.Model.Aggregates;
using CompuCare.Platform.Notifications.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyNotificationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Notification>().HasKey(n => n.Id);
        builder.Entity<Notification>().Property(n => n.Id)
            .HasConversion(new StronglyTypedIdConverter<NotificationId, int>(id => id.Value,
                value => new NotificationId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Notification>().Property(n => n.Message).IsRequired().HasMaxLength(1000);
        builder.Entity<Notification>().HasIndex(n => new { n.UserId, n.CreatedAt });
    }
}
