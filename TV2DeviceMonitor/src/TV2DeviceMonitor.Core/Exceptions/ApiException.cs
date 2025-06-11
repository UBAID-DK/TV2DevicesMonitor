// TV2DeviceMonitor.Core/Exceptions/ApiException.cs
using System;

/// <summary>
/// Represents errors that occur when communicating with the device API.
/// </summary>
public class ApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ApiException(string message) : base(message) { }
}