using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Helper;

namespace Microsoft.Azure.IotHub.Relay.Device
{
    /// <summary>
    /// Provides methods to get a <see cref="DeviceClient"/>.
    /// </summary>
    public class DeviceClientProvider : IDeviceClientProvider
    {
        readonly string _connectionString;
        readonly string _hostname;
        readonly RegistryManager _registryManager;

        public DeviceClientProvider(string connectionString)
        {
            _connectionString = connectionString;
            _hostname = connectionString.GetValue("hostname");
            _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);

        }

        public async Task<DeviceClient> GetDeviceClientAsync(string deviceId, CancellationToken cancellationToken)
        {
            Microsoft.Azure.Devices.Device device = await _registryManager.GetDeviceAsync(deviceId, cancellationToken)
                                                 ?? await _registryManager.AddDeviceAsync(new Microsoft.Azure.Devices.Device(deviceId), cancellationToken);

            DeviceClient deviceClient = DeviceClient.Create(
                hostname: _hostname,
                authenticationMethod: new DeviceAuthenticationWithRegistrySymmetricKey(
                    deviceId: deviceId,
                    key: device.Authentication.SymmetricKey.PrimaryKey
                )
            );

            return deviceClient;
        }
    }
}
