using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Devices.GetById.v1;

public sealed class GetDeviceInformationHandler(AppDbContext dbContext)
    : IQueryHandler<GetDeviceInformationQuery, Result<DeviceInformationDto>>
{
    public async ValueTask<Result<DeviceInformationDto>> Handle(GetDeviceInformationQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.DeviceDetails)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<DeviceInformationDto>(Error.NotFound("Device not found"));
        }

        return Result.Success(new DeviceInformationDto
        {
            DeviceName = device.Name,
            OsName = device.DeviceDetails.OsName,
            OsVersion = device.DeviceDetails.OsVersion,
            Version = device.DeviceDetails.Version,
            ProductName = device.DeviceDetails.ProductName,
            Processor = device.DeviceDetails.Processor,
            InstalledRam = device.DeviceDetails.InstalledRam,
            GraphicsCard = device.DeviceDetails.GraphicsCard,
            Manufacturer = device.DeviceDetails.Manufacturer,
        });
    }
}
