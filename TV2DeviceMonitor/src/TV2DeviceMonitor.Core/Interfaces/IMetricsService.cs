// TV2DeviceMonitor.Core/Interfaces/IMetricsService.cs
using TV2DeviceMonitor.Core.Models;

namespace TV2DeviceMonitor.Core.Interfaces;

public interface IMetricsService
{
    /// <summary>
    /// Reports device metrics to Prometheus.
    /// </summary>
    /// <param name="deviceName">The name of the device.</param>
    /// <param name="state">The current state of the device.</param>
    void ReportMetrics(string deviceName, DeviceState state);
    void SetDeviceStatus(string deviceName, string ip, bool isOnline);
}