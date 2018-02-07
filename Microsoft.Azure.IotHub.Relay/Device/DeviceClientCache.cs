using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Microsoft.Azure.IotHub.Relay.Device
{
    public class DeviceClientCache : IDeviceClientProvider, IDisposable
    {
        readonly IDeviceClientProvider _deviceClientProvider;
        readonly ConcurrentDictionary<string, DeviceClient> _dictionary;

        public DeviceClientCache(IDeviceClientProvider deviceClientProvider)
        {
            _deviceClientProvider = deviceClientProvider;
            _dictionary = new ConcurrentDictionary<string, DeviceClient>(StringComparer.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            ICollection<string> keys = _dictionary.Keys;

            foreach (string key in keys)
            {
                if(_dictionary.TryRemove(key, out DeviceClient deviceClient))
                {
                    deviceClient?.Dispose();
                }
            }
        }

        public Task<DeviceClient> GetDeviceClientAsync(string deviceId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_dictionary.GetOrAdd(deviceId, id => _deviceClientProvider.GetDeviceClientAsync(id, CancellationToken.None).GetAwaiter().GetResult()));
        }
    }
}
