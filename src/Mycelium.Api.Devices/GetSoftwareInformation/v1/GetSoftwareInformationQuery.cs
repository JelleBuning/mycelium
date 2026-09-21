using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.GetSoftwareInformation.v1;

public sealed record GetSoftwareInformationQuery(int DeviceId) : IQuery<Result<SoftwareInformationDto>>;
