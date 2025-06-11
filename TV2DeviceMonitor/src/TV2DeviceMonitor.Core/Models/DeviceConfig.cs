using System.ComponentModel.DataAnnotations; // Add this at the top
namespace TV2DeviceMonitor.Core.Models
{
    /// <summary>
    /// Represents configuration details for a single streaming device.
    /// </summary>
    public class DeviceConfig
    {
        /// <summary>
        /// The friendly name of the device.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The IP address of the device.
        /// </summary>
        [Required]
        public string IpAddress { get; set; }

        /// <summary>
        /// Indicates whether this device is critical and must always be online.
        /// </summary>
        public bool IsCritical { get; set; } = false;

        /// <summary>
        /// The server endpoint (e.g., SRT or RTMP URL) used for streaming.
        /// </summary>
        public string? Server { get; set; }

        /// <summary>
        /// Audio bitrate in bits per second (e.g., 128000).
        /// </summary>
        public int AudioBitrate { get; set; }

        /// <summary>
        /// Video bitrate in bits per second (e.g., 7500000).
        /// </summary>
        public int VideoBitrate { get; set; }

        /// <summary>
        /// Stream resolution (e.g., "1080p", "720p").
        /// </summary>
        public string? Resolution { get; set; }

        /// <summary>
        /// Frames per second (e.g., 25, 50).
        /// </summary>
        public int Fps { get; set; }

        /// <summary>
        /// Video codec used (e.g., "H264").
        /// </summary>
        public string? Codec { get; set; }

        /// <summary>
        /// Optional username for authentication (if needed).
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Optional password for authentication (if needed).
        /// </summary>
        public string? Password { get; set; }
    }
}
