using TV2DeviceMonitor.Core.Models;
using TV2DeviceMonitor.Core.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TV2DeviceMonitor.Core.Interfaces
{
    public interface IConfigurationService
    {
        /// <summary>
        /// Retrieves the list of configured devices from configuration settings.
        /// </summary>
        /// <returns>A list of <see cref="DeviceConfig"/> objects representing device configurations.</returns>
        List<DeviceConfig> GetDevices();
    }

}