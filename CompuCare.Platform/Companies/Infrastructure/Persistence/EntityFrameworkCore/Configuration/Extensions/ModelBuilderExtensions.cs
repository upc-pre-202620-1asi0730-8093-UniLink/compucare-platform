using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Companies.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyCompaniesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Company>().HasKey(c => c.Id);
        builder.Entity<Company>().Property(c => c.Id)
            .HasConversion(new StronglyTypedIdConverter<CompanyId, int>(id => id.Value, value => new CompanyId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Company>().Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Entity<Company>().Property(c => c.TaxId).IsRequired().HasMaxLength(20);
        builder.Entity<Company>().HasIndex(c => c.TaxId).IsUnique();

        builder.Entity<Location>().HasKey(l => l.Id);
        builder.Entity<Location>().Property(l => l.Id)
            .HasConversion(new StronglyTypedIdConverter<LocationId, int>(id => id.Value, value => new LocationId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Location>().Property(l => l.Name).IsRequired().HasMaxLength(200);
        builder.Entity<Location>().Property(l => l.Desk).IsRequired().HasMaxLength(140);
        builder.Entity<Location>().HasIndex(l => new { l.CompanyId, l.Name, l.Desk }).IsUnique();

        builder.Entity<Equipment>().HasKey(e => e.Id);
        builder.Entity<Equipment>().Property(e => e.Id)
            .HasConversion(new StronglyTypedIdConverter<EquipmentId, int>(id => id.Value,
                value => new EquipmentId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Equipment>().Property(e => e.Code).IsRequired().HasMaxLength(30);
        builder.Entity<Equipment>().Property(e => e.Name).IsRequired().HasMaxLength(160);
        builder.Entity<Equipment>().Property(e => e.SerialNumber).IsRequired().HasMaxLength(100);
        builder.Entity<Equipment>().Property(e => e.Type).IsRequired().HasMaxLength(90);
        builder.Entity<Equipment>().HasIndex(e => new { e.CompanyId, e.SerialNumber }).IsUnique();
        builder.Entity<Equipment>().HasIndex(e => e.Code).IsUnique();
        builder.Entity<Equipment>().HasIndex(e => e.AssignedEmployeeId);
        builder.Entity<Equipment>().OwnsMany(e => e.Assignments, assignment =>
        {
            assignment.WithOwner().HasForeignKey("EquipmentId");
            assignment.HasKey(a => a.Id);
            assignment.Property(a => a.Id).ValueGeneratedOnAdd();
        });
        builder.Entity<Equipment>().Navigation(e => e.Assignments).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
