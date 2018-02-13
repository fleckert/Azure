using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.EventHubs.Helper.Demo
{
    class Program
    {
        static async Task Main()
        {
            Microsoft.Azure.EventHubs.Helper.EventProcessorHostWrapper eventProcessorHostWrapper = new Microsoft.Azure.EventHubs.Helper.EventProcessorHostWrapper(
                eventHubConnectionString: @"Endpoint=sb://ihsuprodamres088dednamespace.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=R5M48NLlxESkgZQoOfMhqquJaBzNp0CwtV2rvSVAGwg=;EntityPath=iothub-ehub-fleckert-345306-f14a5f5c63",
                storageConnectionString: @"DefaultEndpointsProtocol=https;AccountName=iothubrelay;AccountKey=fgnzJwaNUuWWybGPaxz9CX6dd6w2Hugcn+r5Rgz8QBGZN5aPRtqn3i9guknbnYQawdCkF8u/prq+GWMrxPMhLw==;EndpointSuffix=core.windows.net",
                leaseContainerName: "demo0",
                getEventProcessor: () => new EventHubForwarderEventProcessor(
                    @"Endpoint=sb://fleckert.servicebus.windows.net/;SharedAccessKeyName=Send;SharedAccessKey=NhKNaJ5dMzgynk3ktBZwS4jLCFq5AeRbdMQG4dy5D5s=;EntityPath=data",
                    _ => _.Properties["iothub-connection-device-id"].ToString(),
                    256 * 1024
                )
            );

            await eventProcessorHostWrapper.RegisterEventProcessorFactoryAsync();

            Console.ReadLine();

            await eventProcessorHostWrapper.UnregisterEventProcessorAsync();
        }
    }
}
