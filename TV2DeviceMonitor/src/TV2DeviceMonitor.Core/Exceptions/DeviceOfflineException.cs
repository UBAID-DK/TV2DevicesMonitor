// TV2DeviceMonitor.Core/Exceptions/DeviceOfflineException.cs
using System;

namespace TV2DeviceMonitor.Core.Exceptions;

/// <summary>
/// Exception thrown when a device is unreachable or considered offline.
/// </summary>
public class DeviceOfflineException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceOfflineException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that describes the offline state.</param>
    public DeviceOfflineException(string message) : base(message) { }
}