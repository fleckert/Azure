using System;
using Microsoft.Azure.EventHubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.EventHubs.Helper
{
    public sealed class EventDataSender : IDisposable
    {
        readonly Func<EventData, string> _getPartitionKey;
        readonly EventHubClient _eventHubClient;
        readonly long _maxSizeInBytes;

        public EventDataSender(string connectionString, Func<EventData, string> getPartitionKey, long maxSizeInBytes)
        {
            _getPartitionKey = getPartitionKey;
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
            _maxSizeInBytes = maxSizeInBytes;
        }

        public async Task SendAsync(IEnumerable<EventData> collection, CancellationToken cancelletionToken)
        {
            Task task = null;

            foreach (IEnumerable<IEnumerable<EventData>> item in ToEventDataBatchesByPartitionKey(collection, _getPartitionKey, _maxSizeInBytes))
            {
                foreach (IEnumerable<EventData> itemsss in item)
                {
                    if(task != null)
                    {
                        await task;
                    }

                    task = _eventHubClient.SendAsync(itemsss, partitionKey : _getPartitionKey(itemsss.First()));
                }
            }
        }

        IEnumerable<IEnumerable<EventData>> ToEventDataBatchesByPartitionKey(IEnumerable<EventData> collection, Func<EventData, string> getPartitionKey, long maxSizeInBytes)
        {
            foreach (var collectionGrouped in collection.GroupBy(p => getPartitionKey(p)))
            {
                foreach (IEnumerable<EventData> eventDataBatch in ToEventDataBatches(collectionGrouped, maxSizeInBytes))
                {
                    yield return eventDataBatch;
                }
            }
        }

        IEnumerable<IEnumerable<EventData>> ToEventDataBatches(IEnumerable<EventData> collection, long maxSizeInBytes)
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
                        throw new InvalidOperationException($"{eventDataBatch.GetType().FullName}.{nameof(eventDataBatch.TryAdd)} failed.");
                    }
                }
            }

            yield return eventDataBatch.ToEnumerable();
        }

        void IDisposable.Dispose()
        {
            _eventHubClient?.Close();
        }
    }
}
