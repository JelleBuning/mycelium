using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Devices.Update.DeviceInformation.v1;

public sealed class UpdateDeviceInformationHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateDeviceInformationCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateDeviceInformationCommand command, CancellationToken cancellationToken)
    {
        var accessError = DeviceAccessGuard.Validate(httpContextAccessor.HttpContext?.User, command.DeviceId);
        if (accessError is not null)
        {
            return Result.Failure(accessError);
        }

        var device = await dbContext.Devices
            .Include(d => d.DeviceDetails)
            .FirstOrDefaultAsync(x => x.Id == command.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure(Error.NotFound("Device not found"));
        }

        var info = command.DeviceInfo;
        device.Name = info.DeviceName ?? device.Name;
        device.DeviceDetails.OsName = info.OsName;
        device.DeviceDetails.OsVersion = info.OsVersion;
        device.DeviceDetails.Version = info.Version;
        device.DeviceDetails.ProductName = info.ProductName;
        device.DeviceDetails.Processor = info.Processor;
        device.DeviceDetails.InstalledRam = info.InstalledRam;
        device.DeviceDetails.GraphicsCard = info.GraphicsCard;
        device.DeviceDetails.Manufacturer = info.Manufacturer;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
