using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IotHub.Relay.Device;

namespace Microsoft.Azure.IotHub.Relay.Controllers
{
    [Route("api/[controller]")]
    public class DeviceController : Controller
    {
        readonly IMessageSender _messageSender;

        public DeviceController(IMessageSender sender)
        {
            _messageSender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] MessageSendRequest messageSendRequest, CancellationToken cancellationToken)
        {
            if (messageSendRequest == null) { return BadRequest(); }
            if (messageSendRequest.Properties == null) { return BadRequest(); }
            if (string.IsNullOrWhiteSpace(messageSendRequest.DeviceId)) { return BadRequest(); }
            if (messageSendRequest.Payload == null) { return BadRequest(); }

            string correlationId = await _messageSender.SendAsync(
                messageSendRequest.DeviceId,
                messageSendRequest.Properties,
                Encoding.UTF8.GetBytes(messageSendRequest.Payload),
                cancellationToken
            );

            return Ok(correlationId);
        }

        public class MessageSendRequest
        {
            public string DeviceId { get; }
            public IDictionary<string, string> Properties { get; }
            public string Payload { get; }

            public MessageSendRequest(string deviceId, IDictionary<string, string> properties, string payload)
            {
                DeviceId = deviceId;
                Properties = properties;
                Payload = payload;
            }
        }
    }
}
