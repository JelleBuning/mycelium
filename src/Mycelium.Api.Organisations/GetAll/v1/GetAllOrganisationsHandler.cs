using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Organisations.GetAll.v1;

public sealed class GetAllOrganisationsHandler(AppDbContext dbContext)
    : IQueryHandler<GetAllOrganisationsQuery, Result<List<OrganisationDto>>>
{
    public async ValueTask<Result<List<OrganisationDto>>> Handle(GetAllOrganisationsQuery query, CancellationToken cancellationToken)
    {
        var organisations = await dbContext.Organisations
            .Select(o => new OrganisationDto
            {
                Id = o.Id,
                Hash = o.Hash,
                Users = o.Users.Select(u => new OrganisationUserDto { Id = u.Id, Email = u.Email }).ToList(),
                Devices = o.Devices.Select(d => new OrganisationDeviceDto { Id = d.Id, Name = d.Name, LastActive = d.LastActive }).ToList()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(organisations);
    }
}
