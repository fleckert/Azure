using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.IotHub.Relay.Device
{
    public interface IMessageSender
    {
        Task<string> SendAsync(string deviceId, IDictionary<string, string> properties, byte[] payload, CancellationToken cancellationToken);
    }
}
