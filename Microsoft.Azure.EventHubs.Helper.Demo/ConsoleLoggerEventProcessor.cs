using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.EventHubs.Helper.Demo
{
    class ConsoleLoggerEventProcessor : IEventProcessor
    {
        Task IEventProcessor.ProcessErrorAsync(PartitionContext partitionContext, Exception error)
        {
            Console.WriteLine($"{nameof(IEventProcessor.ProcessErrorAsync)} {partitionContext.PartitionId} {error}");

            return Task.CompletedTask;
        }

        Task IEventProcessor.CloseAsync(PartitionContext partitionContext, CloseReason reason)
        {
            Console.WriteLine($"{nameof(IEventProcessor.CloseAsync)} {partitionContext.PartitionId} {reason}");

            return Task.CompletedTask;
        }

        Task IEventProcessor.OpenAsync(PartitionContext partitionContext)
        {
            Console.WriteLine($"{nameof(IEventProcessor.OpenAsync)} {partitionContext.PartitionId}");

            return Task.CompletedTask;
        }

        Task IEventProcessor.ProcessEventsAsync(PartitionContext partitionContext, IEnumerable<EventData> messages)
        {
            if (messages != null)
            {
                foreach (EventData eventData in messages)
                {
                    Console.WriteLine($"{nameof(IEventProcessor.ProcessEventsAsync)} {partitionContext.PartitionId} {Encoding.UTF8.GetString(eventData.Body.Array)}");
                }
            }

            return partitionContext.CheckpointAsync();
        }
    }
}
