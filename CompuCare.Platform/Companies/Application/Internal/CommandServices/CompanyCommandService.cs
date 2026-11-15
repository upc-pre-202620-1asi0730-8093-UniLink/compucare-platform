using CompuCare.Platform.Companies.Application.CommandServices;
using CompuCare.Platform.Companies.Domain.Model;
using CompuCare.Platform.Companies.Domain.Model.Aggregates;
using CompuCare.Platform.Companies.Domain.Model.Commands;
using CompuCare.Platform.Companies.Domain.Model.Errors;
using CompuCare.Platform.Companies.Domain.Repositories;
using CompuCare.Platform.Shared.Application.Model;
using CompuCare.Platform.Shared.Domain.Repositories;

namespace CompuCare.Platform.Companies.Application.Internal.CommandServices;

public class CompanyCommandService(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    : ICompanyCommandService
{
    /// <inheritdoc />
    public async Task<Result<Company>> Handle(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        Company company;
        try
        {
            company = new Company(command);
        }
        catch (ArgumentException)
        {
            return Result<Company>.Failure(CompaniesErrors.InvalidCompanyData, CompaniesError.InvalidCompanyData);
        }

        await companyRepository.AddAsync(company, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Company>.Success(company);
    }
}
