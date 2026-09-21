using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.ExecuteSecurityScan.v1;

public sealed record ExecuteSecurityScanCommand(int DeviceId) : ICommand<Result>;
