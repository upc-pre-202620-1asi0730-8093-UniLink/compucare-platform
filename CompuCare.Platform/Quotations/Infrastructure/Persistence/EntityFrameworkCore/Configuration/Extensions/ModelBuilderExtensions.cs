using CompuCare.Platform.Quotations.Domain.Model.Aggregates;
using CompuCare.Platform.Quotations.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.ValueConverters;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Quotations.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyQuotationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Quotation>().HasKey(q => q.Id);
        builder.Entity<Quotation>().Property(q => q.Id)
            .HasConversion(new StronglyTypedIdConverter<QuotationId, int>(id => id.Value,
                value => new QuotationId(value)))
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Quotation>().Property(q => q.Total).HasPrecision(12, 2);
        builder.Entity<Quotation>().Property(q => q.Status).HasConversion<string>().HasMaxLength(35);
        builder.Entity<Quotation>().Property(q => q.Receipt).HasMaxLength(50);
        builder.Entity<Quotation>().Ignore(q => q.IsUnresolved);
        builder.Entity<Quotation>().HasIndex(q => new { q.ServiceRequestId, q.Status });
        builder.Entity<Quotation>().HasIndex(q => new { q.CompanyId, q.Status });
        builder.Entity<Quotation>().OwnsMany(q => q.Items, line =>
        {
            line.WithOwner().HasForeignKey("QuotationId");
            line.HasKey(l => l.Id);
            line.Property(l => l.Id).ValueGeneratedOnAdd();
            line.Property(l => l.Description).IsRequired().HasMaxLength(500);
            line.Property(l => l.Cost).HasPrecision(12, 2);
        });
        builder.Entity<Quotation>().Navigation(q => q.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
