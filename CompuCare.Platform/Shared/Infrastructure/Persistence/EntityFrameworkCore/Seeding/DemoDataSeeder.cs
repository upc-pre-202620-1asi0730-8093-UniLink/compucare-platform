using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Iam.Application.Internal.OutboundServices;
using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Model.ValueObjects;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Seeding;

/// <summary>
///     Optional fictitious accounts for local demonstrations: one per role, a company, a location, an assigned
///     device and a basic subscription. It only runs when <c>DemoSeed:Enabled</c> is true, and the password is
///     read from local configuration, never from the repository.
/// </summary>
public static class DemoDataSeeder
{
    private const string DemoCompanyTaxId = "20123456789";

    public static async Task SeedAsync(AppDbContext context, IHashingService hashingService, string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            throw new InvalidOperationException("DemoSeed__Password must have at least twelve characters.");

        var company = await context.Set<Company>()
            .FirstOrDefaultAsync(c => c.TaxId == DemoCompanyTaxId, cancellationToken);
        if (company is null)
        {
            company = new Company(new CreateCompanyCommand("CompuCare demo company", DemoCompanyTaxId));
            await context.AddAsync(company, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var passwordHash = hashingService.HashPassword(password);

        async Task<User> EnsureUser(string email, string name, string role, int? companyId)
        {
            var existing = await context.Set<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (existing is not null) return existing;
            var user = new User(name, email, passwordHash, role, companyId);
            await context.AddAsync(user, cancellationToken);
            return user;
        }

        await EnsureUser("soporte@unilink.pe", "UniLink Support", UserRoles.SystemAdministrator, null);
        await EnsureUser("tecnico@unilink.pe", "Technical Support", UserRoles.Technician, null);
        await EnsureUser("admin@mype.pe", "Company Administrator", UserRoles.Administrator, company.Id);
        var employee = await EnsureUser("empleada@mype.pe", "Company Employee", UserRoles.Employee, company.Id);
        await context.SaveChangesAsync(cancellationToken);

        var location = await context.Set<Location>()
            .FirstOrDefaultAsync(l => l.CompanyId == company.Id.Value, cancellationToken);
        if (location is null)
        {
            location = new Location(company.Id, "Lima Central Office", "Desk 1");
            await context.AddAsync(location, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.Set<Equipment>().AnyAsync(e => e.CompanyId == company.Id.Value, cancellationToken))
        {
            var device = new Equipment(company.Id, "DEMO-COMPUCARE-001", "Office workstation", "Desktop",
                location.Id);
            device.AssignTo(employee.Id, true);
            await context.AddAsync(device, cancellationToken);
        }

        if (!await context.Set<Subscription>().AnyAsync(s => s.CompanyId == company.Id.Value, cancellationToken))
        {
            var basicPlan = await context.Set<Plan>().FirstAsync(p => p.Code == "basico", cancellationToken);
            await context.AddAsync(new Subscription(company.Id, basicPlan), cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
