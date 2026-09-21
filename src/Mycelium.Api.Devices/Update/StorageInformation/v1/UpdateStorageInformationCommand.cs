using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.StorageInformation.v1;

public sealed record UpdateStorageInformationCommand(int DeviceId, StorageInformationDto StorageInfo) : ICommand<Result>;
