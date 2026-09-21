using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

public class SoftwareInformationRetrieverTests
{
    [Test]
    public void ShouldInclude_NullName_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude(null, false, false), Is.False);
    }

    [Test]
    public void ShouldInclude_WhitespaceName_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("   ", false, false), Is.False);
    }

    [Test]
    public void ShouldInclude_SystemComponent_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("Some App", true, false), Is.False);
    }

    [Test]
    public void ShouldInclude_Update_ReturnsFalse()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("Some App", false, true), Is.False);
    }

    [Test]
    public void ShouldInclude_ValidNameNotSystemComponentNotUpdate_ReturnsTrue()
    {
        Assert.That(SoftwareInformationRetriever.ShouldInclude("Some App", false, false), Is.True);
    }
}
