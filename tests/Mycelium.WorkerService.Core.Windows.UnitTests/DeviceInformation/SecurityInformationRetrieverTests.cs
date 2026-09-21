using Mycelium.WorkerService.Core.Windows.DeviceInformation;
using NUnit.Framework;

namespace Mycelium.WorkerService.Core.Windows.UnitTests.DeviceInformation;

public class SecurityInformationRetrieverTests
{
    [Test]
    public void ParseExact_ValidWmiDateTimeString_ReturnsParsedDateTime()
    {
        var result = SecurityInformationRetriever.ParseExact("20260115103045.123456+000");

        Assert.That(result, Is.EqualTo(new DateTime(2026, 1, 15, 10, 30, 45, 123).AddTicks(4560)));
    }

    [Test]
    public void ParseExact_MalformedInput_Throws()
    {
        Assert.Throws<FormatException>(() => SecurityInformationRetriever.ParseExact("not-a-date"));
    }
}
