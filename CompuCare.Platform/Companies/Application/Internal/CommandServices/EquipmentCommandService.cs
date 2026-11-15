using CompuCare.Platform.Companies.Application.CommandServices;
using CompuCare.Platform.Companies.Application.Internal.OutboundServices.Acl;
using CompuCare.Platform.Companies.Application.Model;
using CompuCare.Platform.Companies.Domain.Model;
using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Domain.Model.Errors;
using CompuCare.Platform.Companies.Domain.Model.ValueObjects;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Companies.Application.Internal.CommandServices;

public class EquipmentCommandService(
    IEquipmentRepository equipmentRepository,
    ILocationRepository locationRepository,
    ExternalEmployeeService externalEmployeeService,
    ExternalServiceRequestService externalServiceRequestService,
    IUnitOfWork unitOfWork)
    : IEquipmentCommandService
{
    /// <inheritdoc />
    public async Task<Result<EquipmentWithStatus>> Handle(CreateEquipmentCommand command,
        CancellationToken cancellationToken)
    {
        Equipment equipment;
        try
        {
            equipment = new Equipment(command);
        }
        catch (ArgumentException)
        {
            return Result<EquipmentWithStatus>.Failure(CompaniesErrors.InvalidEquipmentData,
                CompaniesError.InvalidEquipmentData);
        }

        if (await equipmentRepository.ExistsByCompanyIdAndSerialNumberAsync(command.CompanyId,
                equipment.SerialNumber, cancellationToken))
            return Result<EquipmentWithStatus>.Failure(CompaniesErrors.DuplicateSerialNumber,
                CompaniesError.DuplicateSerialNumber);

        if (command.LocationId.HasValue)
        {
            var location = await locationRepository.FindByIdAsNoTrackingAsync(command.LocationId.Value,
                cancellationToken);
            if (location is null || location.CompanyId != command.CompanyId || !location.Active)
                return Result<EquipmentWithStatus>.Failure(CompaniesErrors.LocationUnavailable,
                    CompaniesError.LocationUnavailable);
        }

        await equipmentRepository.AddAsync(equipment, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<EquipmentWithStatus>.Success(new EquipmentWithStatus(equipment, EEquipmentStatus.Operational));
    }

    /// <inheritdoc />
    public async Task<Result<EquipmentWithStatus>> Handle(AssignEquipmentCommand command,
        CancellationToken cancellationToken)
    {
        var equipment = await equipmentRepository.FindByIdAsync(command.EquipmentId, cancellationToken);
        if (equipment is null || equipment.CompanyId != command.CompanyId)
            return Result<EquipmentWithStatus>.Failure(CompaniesErrors.EquipmentNotFound,
                CompaniesError.EquipmentNotFound);

        if (command.EmployeeId.HasValue)
        {
            var belongsToCompany = await externalEmployeeService.ExistsActiveEmployeeInCompany(
                command.EmployeeId.Value, command.CompanyId, cancellationToken);
            try
            {
                equipment.AssignTo(command.EmployeeId.Value, belongsToCompany);
            }
            catch (InvalidOperationException)
            {
                return Result<EquipmentWithStatus>.Failure(CompaniesErrors.EmployeeNotInCompany,
                    CompaniesError.EmployeeNotInCompany);
            }
        }
        else
        {
            equipment.Unassign();
        }

        await unitOfWork.CompleteAsync(cancellationToken);

        var inRepair = await externalServiceRequestService.FetchEquipmentIdsInRepair(command.CompanyId,
            cancellationToken);
        var status = inRepair.Contains(equipment.Id) ? EEquipmentStatus.InRepair : EEquipmentStatus.Operational;
        return Result<EquipmentWithStatus>.Success(new EquipmentWithStatus(equipment, status));
    }
}
