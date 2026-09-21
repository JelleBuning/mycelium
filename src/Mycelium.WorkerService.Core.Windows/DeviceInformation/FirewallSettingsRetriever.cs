using System.Management;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.Windows.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class FirewallSettingsRetriever : IFirewallSettingsRetriever
{
    public FirewallSettingsDto Retrieve()
    {
        const string firewallProfileScope = @"\\.\root\StandardCimv2";
        const string firewallProfileKey = "MSFT_NetFirewallProfile";
        using var firewallObjectSearcher = new ManagementObjectSearcher(firewallProfileScope, "SELECT * FROM " + firewallProfileKey);
        var netFirewallProfiles = firewallObjectSearcher.Get().Cast<ManagementBaseObject>()
            .Select(x => (Name: x["Name"]?.ToString(), Enabled: x["Enabled"]?.ToString()));

        return MapToFirewallSettings(netFirewallProfiles);
    }

    internal static FirewallSettingsDto MapToFirewallSettings(IEnumerable<(string? Name, string? Enabled)> profiles)
    {
        var profileList = profiles.ToList();

        return new FirewallSettingsDto
        {
            DomainFirewallEnabled = profileList.Single(x => x.Name == "Domain").Enabled == "1",
            PrivateFirewallEnabled = profileList.Single(x => x.Name == "Private").Enabled == "1",
            PublicFirewallEnabled = profileList.Single(x => x.Name == "Public").Enabled == "1",
        };
    }
}