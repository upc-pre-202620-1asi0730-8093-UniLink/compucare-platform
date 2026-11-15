using CompuCare.Platform.Iam.Domain.Model.Aggregates;
using CompuCare.Platform.Iam.Domain.Repositories;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using CompuCare.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CompuCare.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     The password reset token repository.
/// </summary>
/// <param name="context">The database context.</param>
public class PasswordResetTokenRepository(AppDbContext context)
    : BaseRepository<PasswordResetToken>(context), IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await Context.Set<PasswordResetToken>()
            .FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }
}
