using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.GetStorageInformation.v1;

public sealed record GetStorageInformationQuery(int DeviceId) : IQuery<Result<StorageInformationDto>>;
