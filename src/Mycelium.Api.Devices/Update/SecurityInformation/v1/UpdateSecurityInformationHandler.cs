using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.Devices.Common;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Devices.Update.SecurityInformation.v1;

public sealed class UpdateSecurityInformationHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSecurityInformationCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateSecurityInformationCommand command, CancellationToken cancellationToken)
    {
        var accessError = DeviceAccessGuard.Validate(httpContextAccessor.HttpContext?.User, command.DeviceId);
        if (accessError is not null)
        {
            return Result.Failure(accessError);
        }

        var device = await dbContext.Devices
            .Include(d => d.DeviceSecurity)
            .FirstOrDefaultAsync(x => x.Id == command.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure(Error.NotFound("Device not found"));
        }

        var info = command.SecurityInfo;
        device.DeviceSecurity.LastScan = info.LastSecurityScanDto.LastScan;
        device.DeviceSecurity.Duration = info.LastSecurityScanDto.Duration;
        device.DeviceSecurity.AntivirusEnabled = info.AntivirusEnabled;
        device.DeviceSecurity.LastAntivirusUpdate = info.LastAntivirusUpdate;
        device.DeviceSecurity.LastAntispywareUpdate = info.LastAntispywareUpdate;
        device.DeviceSecurity.RealTimeProtectionEnabled = info.RealTimeProtectionEnabled;
        device.DeviceSecurity.NisEnabled = info.NisEnabled;
        device.DeviceSecurity.TamperProtectionEnabled = info.TamperProtectionEnabled;
        device.DeviceSecurity.AntispywareEnabled = info.AntispywareEnabled;
        device.DeviceSecurity.IsVirtualMachine = info.IsVirtualMachine;
        device.DeviceSecurity.DomainFirewallEnabled = info.FirewallSettingsDto.DomainFirewallEnabled;
        device.DeviceSecurity.PrivateFirewallEnabled = info.FirewallSettingsDto.PrivateFirewallEnabled;
        device.DeviceSecurity.PublicFirewallEnabled = info.FirewallSettingsDto.PublicFirewallEnabled;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
