// TV2DeviceMonitor.Core/Interfaces/IApiClient.cs
using TV2DeviceMonitor.Core.Models;
using TV2DeviceMonitor.Core.Exceptions;
using System.Threading.Tasks;

namespace TV2DeviceMonitor.Core.Interfaces;

public interface IApiClient
{
    /// <summary>
    /// Fetches the current state of a device
    /// </summary>
    /// <exception cref="DeviceOfflineException">Thrown when device is unreachable</exception>
    Task<DeviceState?> GetDeviceStateAsync(string ipAddress);

    /// <summary>
    /// Asynchronously authenticates a user by sending their credentials to a specified server.
    /// </summary>
    /// <param name="ipAddress">The IP address of the server where the authentication request will be sent.</param>
    /// <param name="username">The user's username used for authentication.</param>
    /// <param name="password">The user's password used for authentication.</param>
    /// <returns>A task representing the asynchronous authentication operation.</returns>
    Task AuthenticateAsync(string ipAddress, string username, string password);

    Task<bool> StopStreamAsync(string ipAddress);
    Task<bool> StartStreamAsync(string ipAddress);
    Task<bool> UpdateConfigurationAsync(string ipAddress, DeviceConfig config);
    // Add other API methods as needed:
    // Task<DeviceConfig> GetConfigAsync(string ipAddress);
    // Task UpdateConfigAsync(string ipAddress, DeviceConfig config);
}