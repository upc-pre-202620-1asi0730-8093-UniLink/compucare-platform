using CompuCare.Platform.Companies.Application.CommandServices;
using CompuCare.Platform.Companies.Domain.Model;
using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Domain.Model.Errors;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Companies.Application.Internal.CommandServices;

public class LocationCommandService(ILocationRepository locationRepository, IUnitOfWork unitOfWork)
    : ILocationCommandService
{
    /// <inheritdoc />
    public async Task<Result<Location>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        Location location;
        try
        {
            location = new Location(command);
        }
        catch (ArgumentException)
        {
            return Result<Location>.Failure(CompaniesErrors.InvalidLocationData, CompaniesError.InvalidLocationData);
        }

        if (await locationRepository.ExistsByCompanyIdAndNameAndDeskAsync(location.CompanyId, location.Name,
                location.Desk, cancellationToken))
            return Result<Location>.Failure(CompaniesErrors.DuplicateLocation, CompaniesError.DuplicateLocation);

        await locationRepository.AddAsync(location, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Location>.Success(location);
    }

    /// <inheritdoc />
    public async Task<Result<Location>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var location = await locationRepository.FindByIdAsync(command.LocationId, cancellationToken);
        if (location is null || location.CompanyId != command.CompanyId)
            return Result<Location>.Failure(CompaniesErrors.LocationNotFound, CompaniesError.LocationNotFound);

        try
        {
            if (command.Name is not null || command.Desk is not null)
                location.Rename(command.Name ?? location.Name, command.Desk ?? location.Desk);
        }
        catch (ArgumentException)
        {
            return Result<Location>.Failure(CompaniesErrors.InvalidLocationData, CompaniesError.InvalidLocationData);
        }

        if (command.Active.HasValue)
            location.SetActive(command.Active.Value);

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Location>.Success(location);
    }
}
