using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.EventHubs.Helper.Demo
{
    class Program
    {
        static async Task Main()
        {
            Microsoft.Azure.EventHubs.Helper.EventProcessorHostWrapper eventProcessorHostWrapper = new Microsoft.Azure.EventHubs.Helper.EventProcessorHostWrapper(
                eventHubConnectionString: @"Endpoint=sb://<fill_in>.servicebus.windows.net/;SharedAccessKeyName=<fill_in>;SharedAccessKey=<fill_in>;EntityPath=<fill_in>",
                storageConnectionString: @"DefaultEndpointsProtocol=https;AccountName=<fill_in>;AccountKey=<fill_in>;EndpointSuffix=core.windows.net",
                leaseContainerName: "<fill_in>",
                getEventProcessor: () => new ConsoleLoggerEventProcessor()
            );

            await eventProcessorHostWrapper.RegisterEventProcessorFactoryAsync();

            Console.ReadLine();

            await eventProcessorHostWrapper.UnregisterEventProcessorAsync();
        }
    }
}
