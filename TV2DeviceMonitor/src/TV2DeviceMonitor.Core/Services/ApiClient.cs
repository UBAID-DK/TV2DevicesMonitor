using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TV2DeviceMonitor.Core.Models;
using TV2DeviceMonitor.Core.Services; // <-- Add this or correct namespace
using TV2DeviceMonitor.Core.Interfaces;
using System;

namespace TV2DeviceMonitor.Core.Services;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user by sending login credentials to the specified IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address of the server to send the authentication request to.</param>
    /// <param name="username">The username of the user attempting to authenticate.</param>
    /// <param name="password">The password of the user attempting to authenticate.</param>
    /// <returns>A task that represents the asynchronous authentication operation.</returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP response indicates an unsuccessful status code.</exception>
    public async Task AuthenticateAsync(string ipAddress, string username, string password)
    {
        var loginData = new
        {
            username,
            password
        };
        var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"http://{ipAddress}/user/login", content);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Fetches the current state of a device using its IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address of the device.</param>
    /// <returns>The <see cref="DeviceState"/> of the device, or null if the request fails.</returns>
    public async Task<DeviceState?> GetDeviceStateAsync(string ipAddress)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://{ipAddress}/control/api/v1/livestreams/0");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DeviceState>(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get device state from {IpAddress}", ipAddress);
            return null;
        }
    }

    /// <summary>
    /// Sends a request to stop the currently running stream on the specified device.
    /// </summary>
    /// <param name="ipAddress">The IP address of the hardware device.</param>
    /// <returns>True if the stream was stopped successfully; otherwise, false.</returns>
    public async Task<bool> StopStreamAsync(string ipAddress)
    {
        try
        {
            var response = await _httpClient.PutAsync($"http://{ipAddress}/control/api/v1/livestreams/0/stop", null);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;

            _logger.LogWarning("Unexpected response when stopping stream: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop stream on device: {IpAddress}", ipAddress);
            return false;
        }
    }

    /// <summary>
    /// Sends a request to start the stream with the current configuration on the specified device.
    /// </summary>
    /// <param name="ipAddress">The IP address of the hardware device.</param>
    /// <returns>True if the stream was started successfully; otherwise, false.</returns>
    public async Task<bool> StartStreamAsync(string ipAddress)
    {
        try
        {
            var response = await _httpClient.PutAsync($"http://{ipAddress}/control/api/v1/livestreams/0/start", null);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                return true;

            _logger.LogWarning("Unexpected response when starting stream: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start stream on device: {IpAddress}", ipAddress);
            return false;
        }
    }
    /// <summary>
    /// Sends a new configuration to the specified device using a PUT request.
    /// </summary>
    /// <param name="ipAddress">The IP address of the hardware device.</param>
    /// <param name="config">A <see cref="DeviceConfig"/> object representing the desired configuration.</param>
    /// <returns>True if the configuration was updated successfully; otherwise, false.</returns>
    public async Task<bool> UpdateConfigurationAsync(string ipAddress, DeviceConfig config)
    {
        try
        {
            var body = new
            {
                server = config.Server,
                audio_bitrate = config.AudioBitrate,
                video_bitrate = config.VideoBitrate,
                resolution = config.Resolution,
                fps = config.Fps,
                codec = config.Codec
            };

            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://{ipAddress}/control/api/v1/livestreams/customPlatforms/Custom.json", content);
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update configuration for device: {IpAddress}", ipAddress);
            return false;
        }
    }


}
