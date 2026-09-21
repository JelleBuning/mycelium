using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.GetSecurityInformation.v1;

public sealed record GetSecurityInformationQuery(int DeviceId) : IQuery<Result<SecurityInformationDto>>;
