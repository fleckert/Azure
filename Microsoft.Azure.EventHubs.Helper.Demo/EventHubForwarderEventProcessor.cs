using Microsoft.Azure.EventHubs.Processor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.EventHubs.Helper.Demo
{
    sealed class EventHubForwarderEventProcessor : IEventProcessor
    {
        EventDataPartitionedAndBatchedSender _eventDataSender;
        readonly string _connectionString;
        readonly Func<EventData, string> _getPartitionKey;
        readonly long _maxSizeInBytes;


        public EventHubForwarderEventProcessor(string connectionString, Func<EventData, string> getPartitionKey, long maxSizeInBytes)
        {
            _connectionString = connectionString;
            _getPartitionKey = getPartitionKey;
            _maxSizeInBytes = maxSizeInBytes;
        }

        Task IEventProcessor.ProcessErrorAsync(PartitionContext partitionContext, Exception error)
        {
            Console.WriteLine($"{nameof(IEventProcessor.ProcessErrorAsync)} {partitionContext.PartitionId} {error}");

            return Task.CompletedTask;
        }

        Task IEventProcessor.CloseAsync(PartitionContext partitionContext, CloseReason reason)
        {
            Console.WriteLine($"{nameof(IEventProcessor.CloseAsync)} {partitionContext.PartitionId} {reason}");

            _eventDataSender?.Dispose();

            return Task.CompletedTask;
        }

        Task IEventProcessor.OpenAsync(PartitionContext partitionContext)
        {
            Console.WriteLine($"{nameof(IEventProcessor.OpenAsync)} {partitionContext.PartitionId}");

            _eventDataSender = new EventDataPartitionedAndBatchedSender(_connectionString, _getPartitionKey, _maxSizeInBytes);

            return Task.CompletedTask;
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext partitionContext, IEnumerable<EventData> messages)
        {
            await _eventDataSender.SendAsync(messages, CancellationToken.None);

            await partitionContext.CheckpointAsync();
        }
    }
}
