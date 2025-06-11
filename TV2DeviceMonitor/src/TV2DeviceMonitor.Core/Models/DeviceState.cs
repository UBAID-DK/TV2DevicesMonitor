using System.Text.Json.Serialization;

namespace TV2DeviceMonitor.Core.Models
{
    public class DeviceState
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("bitrate")]
        public int Bitrate { get; set; }

        [JsonPropertyName("bufferPct")]
        public int BufferPct { get; set; }
    }
}