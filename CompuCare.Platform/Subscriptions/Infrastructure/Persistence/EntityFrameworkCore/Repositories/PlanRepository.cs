using CompuCare.Platform.Subscriptions.Domain.Model.Aggregates;
using CompuCare.Platform.Subscriptions.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace CompuCare.Platform.Subscriptions.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The plan repository.
/// </summary>
/// <param name="context">The database context.</param>
public class PlanRepository(AppDbContext context) : BaseRepository<Plan>(context), IPlanRepository
{
}
