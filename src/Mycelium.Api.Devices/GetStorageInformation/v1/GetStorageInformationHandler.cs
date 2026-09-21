using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.GetStorageInformation.v1;

public sealed class GetStorageInformationHandler(AppDbContext dbContext)
    : IQueryHandler<GetStorageInformationQuery, Result<StorageInformationDto>>
{
    public async ValueTask<Result<StorageInformationDto>> Handle(GetStorageInformationQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.Disks)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<StorageInformationDto>(Error.NotFound("Device not found"));
        }

        return Result.Success(new StorageInformationDto
        {
            Disks = device.Disks.Select(disk => new DiskInformationDto
            {
                Name = disk.Name,
                IsOsDisk = disk.IsOsDisk,
                Used = disk.Used,
                Size = disk.Size
            }).ToList()
        });
    }
}
