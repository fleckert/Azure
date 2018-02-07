using Microsoft.Azure.Devices.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.IotHub.Relay.Device
{
    public interface IDeviceClientProvider
    {
        Task<DeviceClient> GetDeviceClientAsync(string deviceId, CancellationToken cancellationToken);
    }
}
