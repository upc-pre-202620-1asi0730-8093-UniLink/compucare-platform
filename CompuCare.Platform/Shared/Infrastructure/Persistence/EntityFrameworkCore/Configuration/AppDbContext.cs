using CompuCare.Platform.Companies.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using CompuCare.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using CompuCare.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using CompuCare.Platform.Quotations.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using CompuCare.Platform.ServiceRequests.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using CompuCare.Platform.Subscriptions.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context for the CompuCare Platform
/// </summary>
/// <param name="options">
///     The options for the database context
/// </param>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyIamConfiguration();
        builder.ApplyCompaniesConfiguration();
        builder.ApplySubscriptionsConfiguration();
        builder.ApplyServiceRequestsConfiguration();
        builder.ApplyQuotationsConfiguration();
        builder.ApplyNotificationsConfiguration();

        builder.UseSnakeCaseNamingConvention();
    }
}
