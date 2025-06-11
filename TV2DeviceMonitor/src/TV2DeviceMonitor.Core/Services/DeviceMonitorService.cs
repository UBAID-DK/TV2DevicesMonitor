using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TV2DeviceMonitor.Core.Exceptions;
using TV2DeviceMonitor.Core.Interfaces;
using TV2DeviceMonitor.Core.Models;

namespace TV2DeviceMonitor.Core.Services
{
    public class DeviceMonitorService : BackgroundService
    {
        private readonly IApiClient _apiClient;
        private readonly IMetricsService _metricsService;
        private readonly ILogger<DeviceMonitorService> _logger;
        private readonly List<DeviceConfig>? _devices;

        public DeviceMonitorService(
            IApiClient apiClient,
            IMetricsService metricsService,
            ILogger<DeviceMonitorService> logger,
            IConfiguration configuration)
        {
            _apiClient = apiClient;
            _metricsService = metricsService;
            _logger = logger;
            _devices = configuration.GetSection("Devices").Get<List<DeviceConfig>>();

            if (_devices == null || !_devices.Any())
            {
                throw new InvalidOperationException("No devices configured in 'Devices' section.");
            }
        }

        /// <summary>
        /// Executes the background monitoring loop that periodically checks the state of configured devices.
        /// </summary>
        /// <param name="stoppingToken">Token used to cancel the background task.</param>
        /// <returns>A task representing the asynchronous monitoring loop.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_devices == null || !_devices.Any())
            {
                _logger.LogWarning("No devices found to monitor. Skipping monitoring loop.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var device in _devices)
                {
                    if (string.IsNullOrWhiteSpace(device.IpAddress) || string.IsNullOrWhiteSpace(device.Name))
                    {
                        _logger.LogWarning("Skipping device due to missing IP address or name.");
                        continue;
                    }

                    try
                    {
                        var state = await _apiClient.GetDeviceStateAsync(device.IpAddress);
                        if (state != null)
                        {
                            _metricsService.ReportMetrics(device.Name, state);
                            _metricsService.SetDeviceStatus(device.Name, device.IpAddress, true);
                        }

                        // Use device.Username and device.Password from the config
                        bool success = await PerformDeviceUpdateAsync(device, device.Username, device.Password);
                        if (success)
                        {
                            _logger.LogInformation("Device {DeviceName} updated successfully.", device.Name);
                        }
                        else
                        {
                            _logger.LogWarning("Device {DeviceName} update failed.", device.Name);
                            _metricsService.SetDeviceStatus(device.Name, device.IpAddress, false);
                        }
                    }
                    catch (DeviceOfflineException ex)
                    {
                        _logger.LogWarning(ex, $"Device offline: {device.Name} at {device.IpAddress}");

                        if (device.IsCritical)
                        {
                            // TODO: send alert (Slack, email, etc.)
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        _logger.LogWarning(ex, $"HTTP request failed for {device.Name} at {device.IpAddress}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Unexpected error for {device.Name} at {device.IpAddress}");
                    }
                }

                Console.WriteLine("Monitoring...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task<bool> PerformDeviceUpdateAsync(DeviceConfig device, string? user, string? pass)
        {
            // Null check for required parameters
            if (device == null)
            {
                _logger.LogError("Device configuration cannot be null");
                return false;
            }

            if (string.IsNullOrEmpty(user))
            {
                _logger.LogError("Username cannot be null or empty for device {DeviceName}", device.Name);
                return false;
            }

            if (string.IsNullOrEmpty(pass))
            {
                _logger.LogError("Password cannot be null or empty for device {DeviceName}", device.Name);
                return false;
            }

            if (string.IsNullOrEmpty(device.IpAddress))
            {
                _logger.LogError("IP address cannot be null or empty for device {DeviceName}", device.Name);
                return false;
            }

            try
            {
                await _apiClient.AuthenticateAsync(device.IpAddress, user, pass);

                bool stopped = await _apiClient.StopStreamAsync(device.IpAddress);
                if (!stopped)
                {
                    _logger.LogWarning("Failed to stop stream on device {DeviceName}", device.Name);
                    return false;
                }

                bool updated = await _apiClient.UpdateConfigurationAsync(device.IpAddress, device);
                if (!updated)
                {
                    _logger.LogWarning("Failed to update configuration on device {DeviceName}", device.Name);
                    return false;
                }

                bool started = await _apiClient.StartStreamAsync(device.IpAddress);
                if (!started)
                {
                    _logger.LogWarning("Failed to start stream on device {DeviceName}", device.Name);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing device update for {DeviceName}", device.Name);
                return false;
            }
        }
    }
}
