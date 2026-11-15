using CompuCare.Platform.ServiceRequests.Domain.Model.Aggregates;
using CompuCare.Platform.ServiceRequests.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.ServiceRequests.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyServiceRequestsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<ServiceRequest>().HasKey(r => r.Id);
        builder.Entity<ServiceRequest>().Property(r => r.Id)
            .HasConversion(new StronglyTypedIdConverter<ServiceRequestId, int>(id => id.Value,
                value => new ServiceRequestId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<ServiceRequest>().Property(r => r.Category).IsRequired().HasMaxLength(100);
        builder.Entity<ServiceRequest>().Property(r => r.Description).IsRequired().HasMaxLength(2000);
        builder.Entity<ServiceRequest>().Property(r => r.Diagnosis).HasMaxLength(4000);
        builder.Entity<ServiceRequest>().Property(r => r.WorkDone).HasMaxLength(4000);
        builder.Entity<ServiceRequest>().Property(r => r.WorkHours).HasPrecision(10, 2);
        builder.Entity<ServiceRequest>().Property(r => r.Status).HasConversion<string>().HasMaxLength(35);
        builder.Entity<ServiceRequest>().Ignore(r => r.IsOpen);
        builder.Entity<ServiceRequest>().Ignore(r => r.AcceptsQuotations);
        builder.Entity<ServiceRequest>().HasIndex(r => new { r.CompanyId, r.Status });
        builder.Entity<ServiceRequest>().HasIndex(r => r.TechnicianId);
        builder.Entity<ServiceRequest>().HasIndex(r => r.EquipmentId);
        builder.Entity<ServiceRequest>().OwnsMany(r => r.Timeline, progress =>
        {
            progress.WithOwner().HasForeignKey("ServiceRequestId");
            progress.HasKey(p => p.Id);
            progress.Property(p => p.Id).ValueGeneratedOnAdd();
            progress.Property(p => p.Status).HasConversion<string>().HasMaxLength(35);
        });
        builder.Entity<ServiceRequest>().Navigation(r => r.Timeline).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
