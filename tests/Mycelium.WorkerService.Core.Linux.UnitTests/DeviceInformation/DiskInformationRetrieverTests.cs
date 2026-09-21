using Mycelium.WorkerService.Core.Linux.DeviceInformation;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Linux.UnitTests.DeviceInformation;

public class DiskInformationRetrieverTests
{
    [Test]
    public void ParseSmartctlHealth_PassedTrue_ReturnsOk()
    {
        const string json = """{"smart_status":{"passed":true}}""";

        var result = DiskInformationRetriever.ParseSmartctlHealth(json);

        Assert.That(result, Is.EqualTo("OK"));
    }

    [Test]
    public void ParseSmartctlHealth_PassedFalse_ReturnsFailing()
    {
        const string json = """{"smart_status":{"passed":false}}""";

        var result = DiskInformationRetriever.ParseSmartctlHealth(json);

        Assert.That(result, Is.EqualTo("Failing"));
    }

    [Test]
    public void ParseSmartctlHealth_MissingSmartStatusProperty_ReturnsNull()
    {
        const string json = """{"other_property":123}""";

        var result = DiskInformationRetriever.ParseSmartctlHealth(json);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseSmartctlHealth_EmptyString_ReturnsNull()
    {
        var result = DiskInformationRetriever.ParseSmartctlHealth(string.Empty);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseSmartctlHealth_WhitespaceString_ReturnsNull()
    {
        var result = DiskInformationRetriever.ParseSmartctlHealth("   ");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ParseSmartctlHealth_MalformedJson_Throws()
    {
        Assert.That(() => DiskInformationRetriever.ParseSmartctlHealth("not-json"),
            Throws.InstanceOf<System.Text.Json.JsonException>());
    }

    [Test]
    public void FindDeviceForMountPoint_MatchFound_ReturnsDevice()
    {
        var lines = new[]
        {
            "/dev/sda1 / ext4 rw,relatime 0 0",
            "/dev/sda2 /boot ext4 rw,relatime 0 0"
        };

        var result = DiskInformationRetriever.FindDeviceForMountPoint(lines, "/boot");

        Assert.That(result, Is.EqualTo("/dev/sda2"));
    }

    [Test]
    public void FindDeviceForMountPoint_NoMatch_ReturnsNull()
    {
        var lines = new[] { "/dev/sda1 / ext4 rw,relatime 0 0" };

        var result = DiskInformationRetriever.FindDeviceForMountPoint(lines, "/data");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void FindDeviceForMountPoint_LineWithFewerThanTwoFields_IsSkipped()
    {
        var lines = new[] { "malformed-line", "/dev/sda1 / ext4 rw,relatime 0 0" };

        var result = DiskInformationRetriever.FindDeviceForMountPoint(lines, "/");

        Assert.That(result, Is.EqualTo("/dev/sda1"));
    }
}
