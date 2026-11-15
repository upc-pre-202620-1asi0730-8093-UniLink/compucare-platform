using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CompuCare.Platform.Subscriptions.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySubscriptionsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Plan>().HasKey(p => p.Id);
        builder.Entity<Plan>().Property(p => p.Id)
            .HasConversion(new StronglyTypedIdConverter<PlanId, int>(id => id.Value, value => new PlanId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Plan>().Property(p => p.Code).IsRequired().HasMaxLength(60);
        builder.Entity<Plan>().Property(p => p.Name).IsRequired().HasMaxLength(90);
        builder.Entity<Plan>().Property(p => p.Price).HasPrecision(12, 2);
        builder.Entity<Plan>().Property(p => p.Hours).HasPrecision(10, 2);
        builder.Entity<Plan>().HasIndex(p => p.Code).IsUnique();
        // Published catalog, matching the public landing page.
        builder.Entity<Plan>().HasData(
            new { Id = new PlanId(1), Code = "basico", Name = "Básico", Price = 240m, Hours = 4m, PreventiveVisits = 1 },
            new { Id = new PlanId(2), Code = "empresarial", Name = "Empresarial", Price = 550m, Hours = 8m, PreventiveVisits = 2 },
            new { Id = new PlanId(3), Code = "integral", Name = "Integral", Price = 600m, Hours = 16m, PreventiveVisits = 4 });

        builder.Entity<Subscription>().HasKey(s => s.Id);
        builder.Entity<Subscription>().Property(s => s.Id)
            .HasConversion(new StronglyTypedIdConverter<SubscriptionId, int>(id => id.Value,
                value => new SubscriptionId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Subscription>().Property(s => s.PlanId)
            .HasConversion(new StronglyTypedIdConverter<PlanId, int>(id => id.Value, value => new PlanId(value)))
            .IsRequired();
        builder.Entity<Subscription>().Property(s => s.PlanName).IsRequired().HasMaxLength(90);
        builder.Entity<Subscription>().Property(s => s.MonthlyHours).HasPrecision(10, 2);
        builder.Entity<Subscription>().Property(s => s.UsedHours).HasPrecision(10, 2);
        builder.Entity<Subscription>().Property(s => s.Status).IsRequired().HasMaxLength(20);
        builder.Entity<Subscription>().Property(s => s.Version).IsConcurrencyToken();
        builder.Entity<Subscription>().Ignore(s => s.AvailableHours);
        builder.Entity<Subscription>().Ignore(s => s.PreventiveRemaining);
        builder.Entity<Subscription>().Ignore(s => s.PercentUsed);
        builder.Entity<Subscription>().Ignore(s => s.IsExpired);
        builder.Entity<Subscription>().HasIndex(s => s.CompanyId).IsUnique();
        builder.Entity<Subscription>().OwnsMany(s => s.Consumptions, consumption =>
        {
            consumption.WithOwner().HasForeignKey("SubscriptionId");
            consumption.HasKey(c => c.Id);
            consumption.Property(c => c.Id).ValueGeneratedOnAdd();
            consumption.Property(c => c.Hours).HasPrecision(10, 2);
            consumption.HasIndex("SubscriptionId", nameof(Domain.Model.Entities.CoverageConsumption.ServiceRequestId),
                    nameof(Domain.Model.Entities.CoverageConsumption.PeriodStartedAt)).IsUnique()
                // MySQL identifiers are limited to 64 characters, so the generated name is shortened.
                .HasDatabaseName("IXCoverageConsumptionsRequestPeriod");
        });
        builder.Entity<Subscription>().Navigation(s => s.Consumptions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Entity<MaintenanceVisit>().HasKey(v => v.Id);
        builder.Entity<MaintenanceVisit>().Property(v => v.Id)
            .HasConversion(new StronglyTypedIdConverter<MaintenanceVisitId, int>(id => id.Value,
                value => new MaintenanceVisitId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<MaintenanceVisit>().Property(v => v.SubscriptionId)
            .HasConversion(new StronglyTypedIdConverter<SubscriptionId, int>(id => id.Value,
                value => new SubscriptionId(value)))
            .IsRequired();
        builder.Entity<MaintenanceVisit>().Property(v => v.Date).IsRequired().HasMaxLength(10);
        builder.Entity<MaintenanceVisit>().Property(v => v.Time).IsRequired().HasMaxLength(8);
        builder.Entity<MaintenanceVisit>().Property(v => v.Status).IsRequired().HasMaxLength(40);
        // MySQL has no array columns: the device ids are stored as a comma-separated list.
        builder.Entity<MaintenanceVisit>().Property(v => v.EquipmentIds)
            .HasConversion(
                ids => string.Join(',', ids),
                text => text.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
                new ValueComparer<List<int>>(
                    (left, right) => left!.SequenceEqual(right!),
                    ids => ids.Aggregate(0, (hash, id) => HashCode.Combine(hash, id)),
                    ids => ids.ToList()))
            .HasMaxLength(2000)
            .IsRequired();
        builder.Entity<MaintenanceVisit>().HasIndex(v => new { v.CompanyId, v.Date });
    }
}
