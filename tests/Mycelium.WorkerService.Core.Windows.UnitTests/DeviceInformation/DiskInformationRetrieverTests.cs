using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

public class DiskInformationRetrieverTests
{
    [Test]
    public void MapToDiskDto_TrimsTrailingBackslashFromDriveName()
    {
        var health = new Dictionary<string, string?>();

        var result = DiskInformationRetriever.MapToDiskDto(@"C:\", 500, 250, true, health);

        Assert.That(result.Name, Is.EqualTo("C:"));
    }

    [Test]
    public void MapToDiskDto_HealthStatusPresentForTrimmedName_IsAssigned()
    {
        var health = new Dictionary<string, string?> { ["C:"] = "OK" };

        var result = DiskInformationRetriever.MapToDiskDto(@"C:\", 500, 250, true, health);

        Assert.That(result.HealthStatus, Is.EqualTo("OK"));
    }

    [Test]
    public void MapToDiskDto_DriveNotInHealthDictionary_HealthStatusIsNull()
    {
        var health = new Dictionary<string, string?> { ["D:"] = "OK" };

        var result = DiskInformationRetriever.MapToDiskDto(@"C:\", 500, 250, true, health);

        Assert.That(result.HealthStatus, Is.Null);
    }

    [Test]
    public void MapToDiskDto_PassesThroughSizeUsedAndIsOsDisk()
    {
        var health = new Dictionary<string, string?>();

        var result = DiskInformationRetriever.MapToDiskDto(@"D:\", 1000, 400, false, health);

        Assert.That(result.Size, Is.EqualTo(1000));
        Assert.That(result.Used, Is.EqualTo(400));
        Assert.That(result.IsOsDisk, Is.False);
    }
}
