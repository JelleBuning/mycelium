using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.Ping.v1;

public sealed record PingDeviceCommand(int DeviceId) : ICommand<Result>;
