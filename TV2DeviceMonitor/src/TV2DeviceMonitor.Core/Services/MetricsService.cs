// TV2DeviceMonitor.Core/Services/MetricsService.cs
using System.Diagnostics.Metrics;
using Prometheus;
using TV2DeviceMonitor.Core.Interfaces;
using TV2DeviceMonitor.Core.Models;

namespace TV2DeviceMonitor.Core.Services;

public class MetricsService : IMetricsService
{
    private static readonly Gauge _statusGauge = Metrics
        .CreateGauge("tv2_device_status", "0=Offline, 1=Idle, 2=Streaming", new GaugeConfiguration
        {
            LabelNames = new[] { "device" }
        });

    private static readonly Gauge _bitrateGauge = Metrics
        .CreateGauge("tv2_device_bitrate", "Stream bitrate (bps)", new GaugeConfiguration
        {
            LabelNames = new[] { "device" }
        });

    private static readonly Gauge _bufferGauge = Metrics
        .CreateGauge("tv2_device_buffer_pct", "Stream buffer percentage", new GaugeConfiguration
        {
            LabelNames = new[] { "device" }
        });

    private static readonly Gauge DeviceOnline = Metrics.CreateGauge(
        "tv2_device_online",
        "Device online status (1 = online, 0 = offline)",
        new GaugeConfiguration
        {
            LabelNames = new[] { "device_name", "ip" }
        });

    public void SetDeviceStatus(string deviceName, string ip, bool isOnline)
    {
        DeviceOnline.WithLabels(deviceName, ip).Set(isOnline ? 1 : 0);
    }


    /// <summary>
    /// Updates Prometheus metrics for a given device based on its current state.
    /// </summary>
    /// <param name="deviceName">The name of the device.</param>
    /// <param name="state">The current state of the device, including status and bitrate.</param>
    public void ReportMetrics(string deviceName, DeviceState state)
    {
        _statusGauge.WithLabels(deviceName).Set(state.Status switch
        {
            "Idle" => 1,
            "Streaming" => 2,
            "Interrupted" => -1,
            _ => 0
        });

        _bitrateGauge.WithLabels(deviceName).Set(state.BufferPct);
    }
}