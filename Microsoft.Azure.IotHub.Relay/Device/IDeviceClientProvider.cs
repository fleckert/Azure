using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Microsoft.Azure.IotHub.Relay.Device
{
    public interface IDeviceClientProvider
    {
        Task<DeviceClient> GetDeviceClientAsync(string deviceId, CancellationToken cancellationToken);
    }
}
