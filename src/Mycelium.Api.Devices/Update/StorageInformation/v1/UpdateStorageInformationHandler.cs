using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;
using Mycelium.Api.EntityFramework.Entities;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Devices.Update.StorageInformation.v1;

public sealed class UpdateStorageInformationHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateStorageInformationCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateStorageInformationCommand command, CancellationToken cancellationToken)
    {
        var accessError = DeviceAccessGuard.Validate(httpContextAccessor.HttpContext?.User, command.DeviceId);
        if (accessError is not null)
        {
            return Result.Failure(accessError);
        }

        var device = await dbContext.Devices
            .Include(d => d.Disks)
            .FirstOrDefaultAsync(x => x.Id == command.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure(Error.NotFound("Device not found"));
        }

        foreach (var updateDisk in command.StorageInfo.Disks)
        {
            var disk = device.Disks.FirstOrDefault(d => d.Name == updateDisk.Name);
            if (disk is null)
            {
                device.Disks.Add(new DeviceDisk
                {
                    Name = updateDisk.Name,
                    Size = updateDisk.Size,
                    IsOsDisk = updateDisk.IsOsDisk,
                    Used = updateDisk.Used
                });
            }
            else
            {
                disk.Name = updateDisk.Name;
                disk.Size = updateDisk.Size;
                disk.IsOsDisk = updateDisk.IsOsDisk;
                disk.Used = updateDisk.Used;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
