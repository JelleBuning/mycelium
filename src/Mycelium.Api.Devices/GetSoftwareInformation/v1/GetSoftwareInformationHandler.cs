using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.GetSoftwareInformation.v1;

public sealed class GetSoftwareInformationHandler(AppDbContext dbContext)
    : IQueryHandler<GetSoftwareInformationQuery, Result<SoftwareInformationDto>>
{
    public async ValueTask<Result<SoftwareInformationDto>> Handle(GetSoftwareInformationQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.Software)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<SoftwareInformationDto>(Error.NotFound("Device not found"));
        }

        return Result.Success(new SoftwareInformationDto
        {
            Software = device.Software
                .Select(software => new SoftwareDto { Name = software.Name })
                .OrderBy(software => software.Name)
                .ToList()
        });
    }
}
