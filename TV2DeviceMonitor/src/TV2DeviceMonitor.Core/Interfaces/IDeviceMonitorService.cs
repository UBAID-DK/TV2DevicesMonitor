// TV2DeviceMonitor.Core/Interfaces/IDeviceMonitorService.cs
using System.Threading;
using System.Threading.Tasks;

namespace TV2DeviceMonitor.Core.Interfaces;

public interface IDeviceMonitorService
{
    /// <summary>
    /// Starts monitoring devices asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to signal the cancellation of the monitoring operation.</param>
    /// <returns>A task representing the asynchronous monitoring operation.</returns>
    Task StartMonitoringAsync(CancellationToken cancellationToken);
}