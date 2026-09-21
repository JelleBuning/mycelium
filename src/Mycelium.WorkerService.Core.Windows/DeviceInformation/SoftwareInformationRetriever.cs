using Microsoft.Win32;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class SoftwareInformationRetriever : ISoftwareInformationRetriever
{
    private static readonly (RegistryKey Hive, string SubKey)[] UninstallKeys =
    [
        (Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
        (Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"),
        (Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
    ];

    public List<SoftwareDto> Retrieve()
    {
        return UninstallKeys
            .SelectMany(x => ReadNames(x.Hive, x.SubKey))
            .Distinct()
            .Select(name => new SoftwareDto { Name = name })
            .ToList();
    }

    private static IEnumerable<string> ReadNames(RegistryKey hive, string subKeyPath)
    {
        using var uninstallKey = hive.OpenSubKey(subKeyPath);
        if (uninstallKey is null)
        {
            yield break;
        }

        foreach (var subKeyName in uninstallKey.GetSubKeyNames())
        {
            using var subKey = uninstallKey.OpenSubKey(subKeyName);
            var name = subKey?.GetValue("DisplayName") as string;
            var isSystemComponent = subKey?.GetValue("SystemComponent") is int systemComponent && systemComponent == 1;
            var isUpdate = subKey?.GetValue("ParentKeyName") is not null;

            if (!ShouldInclude(name, isSystemComponent, isUpdate))
            {
                continue;
            }

            yield return name!;
        }
    }

    internal static bool ShouldInclude(string? name, bool isSystemComponent, bool isUpdate)
    {
        return !string.IsNullOrWhiteSpace(name) && !isSystemComponent && !isUpdate;
    }
}
