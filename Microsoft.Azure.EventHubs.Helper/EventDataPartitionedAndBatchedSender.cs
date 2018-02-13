using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.EventHubs.Helper
{
    public sealed class EventDataPartitionedAndBatchedSender : IDisposable
    {
        readonly Func<EventData, string> _getPartitionKey;
        readonly EventHubClient _eventHubClient;
        readonly long _maxSizeInBytes;

        public EventDataPartitionedAndBatchedSender(string connectionString, Func<EventData, string> getPartitionKey, long maxSizeInBytes)
        {
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
            _getPartitionKey = getPartitionKey;
            _maxSizeInBytes = maxSizeInBytes;
        }

        public async Task SendAsync(IEnumerable<EventData> collection, CancellationToken cancellationToken)
        {
            foreach (IEnumerable<EventData> collectionBatched in GetEventDataBatchesGroupedByPartitionKey(collection, _getPartitionKey, _maxSizeInBytes))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _eventHubClient.SendAsync(collectionBatched, partitionKey: _getPartitionKey(collectionBatched.First()));
            }
        }

        public void Dispose()
        {
            _eventHubClient?.Close();
        }

        IEnumerable<IEnumerable<EventData>> GetEventDataBatchesGroupedByPartitionKey(IEnumerable<EventData> collection, Func<EventData, string> getPartitionKey, long maxSizeInBytes)
        {
            foreach (var collectionGrouped in collection.GroupBy(p => getPartitionKey(p)))
            {
                foreach (IEnumerable<EventData> eventDataBatch in GetEventDataBatches(collectionGrouped, maxSizeInBytes))
                {
                    yield return eventDataBatch;
                }
            }
        }

        IEnumerable<IEnumerable<EventData>> GetEventDataBatches(IEnumerable<EventData> collection, long maxSizeInBytes)
        {
            EventDataBatch eventDataBatch = new EventDataBatch(maxSizeInBytes);

            foreach (var item in collection)
            {
                if (!eventDataBatch.TryAdd(item))
                {
                    // add did not happen, create new batch and add
                    yield return eventDataBatch.ToEnumerable();

                    eventDataBatch = new EventDataBatch(maxSizeInBytes);
                   
                    if (!eventDataBatch.TryAdd(item))
                    {
                        throw new Exception($"{eventDataBatch.GetType().FullName}.{nameof(eventDataBatch.TryAdd)} failed.");
                    }
                }
            }

            yield return eventDataBatch.ToEnumerable();
        }
    }
}
