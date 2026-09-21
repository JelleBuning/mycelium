using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.RequestRemoteAccess.v1;

public sealed record RequestRemoteAccessCommand(int DeviceId) : ICommand<Result>;
