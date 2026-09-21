using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.GetSecurityInformation.v1;

public sealed class GetSecurityInformationHandler(AppDbContext dbContext)
    : IQueryHandler<GetSecurityInformationQuery, Result<SecurityInformationDto>>
{
    public async ValueTask<Result<SecurityInformationDto>> Handle(GetSecurityInformationQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.DeviceSecurity)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<SecurityInformationDto>(Error.NotFound("Device not found"));
        }

        var info = device.DeviceSecurity;
        return Result.Success(new SecurityInformationDto
        {
            LastSecurityScanDto = new LastSecurityScanDto
            {
                LastScan = info.LastScan,
                Duration = info.Duration
            },
            AntivirusEnabled = info.AntivirusEnabled,
            LastAntivirusUpdate = info.LastAntivirusUpdate,
            LastAntispywareUpdate = info.LastAntispywareUpdate,
            RealTimeProtectionEnabled = info.RealTimeProtectionEnabled,
            NisEnabled = info.NisEnabled,
            TamperProtectionEnabled = info.TamperProtectionEnabled,
            AntispywareEnabled = info.AntispywareEnabled,
            IsVirtualMachine = info.IsVirtualMachine,
            FirewallSettingsDto = new FirewallSettingsDto
            {
                DomainFirewallEnabled = info.DomainFirewallEnabled,
                PrivateFirewallEnabled = info.PrivateFirewallEnabled,
                PublicFirewallEnabled = info.PublicFirewallEnabled
            }
        });
    }
}
