using Mediator;
using Mycelium.Api.Core.Results;

namespace Mycelium.Api.Devices.GetAll.v1;

public sealed record GetAllDevicesQuery(int UserId) : IQuery<Result<DevicesResponse>>;
