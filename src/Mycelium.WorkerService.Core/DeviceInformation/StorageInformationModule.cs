using Microsoft.Extensions.Logging;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.Module;
using Mycelium.WorkerService.Common.Module.Interfaces;
using Mycelium.WorkerService.Core.DeviceInformation.Interfaces;

namespace Mycelium.WorkerService.Core.DeviceInformation;

public class StorageInformationModule(
    ILogger<StorageInformationModule> logger,
    IScheduledModuleConfig<StorageInformationModule> config,
    IStorageInformationRetriever storageInformationRetriever,
    MyceliumApiService myceliumApiService)
    : ScheduledModuleBase<StorageInformationModule>(logger, config)
{
    public override async Task Execute(CancellationToken cancellationToken)
    {
        var storageInfo = storageInformationRetriever.Retrieve();
        await myceliumApiService.UpdateStorageInformationAsync(storageInfo);
    }
}