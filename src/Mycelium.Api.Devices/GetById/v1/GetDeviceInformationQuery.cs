using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Devices.GetById.v1;

public sealed record GetDeviceInformationQuery(int DeviceId) : IQuery<Result<DeviceInformationDto>>;
