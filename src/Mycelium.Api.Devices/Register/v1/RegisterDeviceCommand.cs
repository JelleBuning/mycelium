using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Register.v1;

public sealed record RegisterDeviceCommand(Guid OrganisationHash, string Name) : ICommand<Result<DeviceTokenResponse>>;
