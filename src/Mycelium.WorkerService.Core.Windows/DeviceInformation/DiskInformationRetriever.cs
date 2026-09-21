using System.Management;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.Windows.DeviceInformation;

#pragma warning disable CA1416
public class DiskInformationRetriever : IDiskInformationRetriever
{
    public List<DiskDto> Retrieve()
    {
        var osDir = Path.GetPathRoot(Environment.SystemDirectory);
        var health = GetHealthByDriveLetter();

        return DriveInfo.GetDrives()
            .Select(x => MapToDiskDto(x.Name, x.TotalSize, x.TotalSize - x.TotalFreeSpace, osDir == x.Name, health))
            .ToList();
    }

    internal static DiskDto MapToDiskDto(string driveName, double size, double used, bool isOsDisk, Dictionary<string, string?> health)
    {
        var name = driveName.TrimEnd('\\');
        health.TryGetValue(name, out var status);

        return new DiskDto
        {
            Name = name,
            Size = size,
            Used = used,
            IsOsDisk = isOsDisk,
            HealthStatus = status
        };
    }

    private static Dictionary<string, string?> GetHealthByDriveLetter()
    {
        var result = new Dictionary<string, string?>();

        try
        {
            using var diskDriveSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (var diskDrive in diskDriveSearcher.Get().Cast<ManagementObject>())
            {
                try
                {
                    var deviceId = diskDrive["DeviceID"]?.ToString();
                    var status = diskDrive["Status"]?.ToString();
                    if (deviceId is null)
                    {
                        continue;
                    }

                    foreach (var driveLetter in GetDriveLettersForDisk(deviceId))
                    {
                        result[driveLetter] = status;
                    }
                }
                catch
                {
                    // ignored - health data is best-effort per disk
                }
            }
        }
        catch
        {
            // ignored - health data is best-effort
        }

        return result;
    }

    private static IEnumerable<string> GetDriveLettersForDisk(string diskDeviceId)
    {
        using var partitionSearcher = new ManagementObjectSearcher(
            $"ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{diskDeviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition");

        foreach (var partition in partitionSearcher.Get().Cast<ManagementObject>())
        {
            using var logicalDiskSearcher = new ManagementObjectSearcher(
                $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_LogicalDiskToPartition");

            foreach (var logicalDisk in logicalDiskSearcher.Get().Cast<ManagementObject>())
            {
                var driveLetter = logicalDisk["DeviceID"]?.ToString();
                if (driveLetter is not null)
                {
                    yield return driveLetter;
                }
            }
        }
    }
}
