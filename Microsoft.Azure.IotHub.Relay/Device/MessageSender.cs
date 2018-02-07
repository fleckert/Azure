using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Microsoft.Azure.IotHub.Relay.Device
{
    public class MessageSender : IMessageSender
    {
        readonly IDeviceClientProvider _deviceClientProvider;

        public MessageSender(IDeviceClientProvider deviceClientProvider)
        {
            _deviceClientProvider = deviceClientProvider;
        }

        public async Task<string> SendAsync(string deviceId, IDictionary<string, string> properties, byte[] payload, CancellationToken cancellationToken)
        {
            DeviceClient deviceClient = await _deviceClientProvider.GetDeviceClientAsync(deviceId, cancellationToken).ConfigureAwait(false);

            Message message = new Message(payload)
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            foreach (var item in properties)
            {
                message.Properties[item.Key] = item.Value;
            }

            await deviceClient.SendEventAsync(message);

            return message.CorrelationId;
        }
    }
}
