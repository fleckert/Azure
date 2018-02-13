using System;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading.Tasks;

namespace Microsoft.Azure.EventHubs.Helper
{
    public sealed class EventProcessorHostWrapper
    {
        readonly EventProcessorHost _eventProcessorHost;
        readonly EventProcessorFactory _eventProcessorFactory;
        readonly EventProcessorOptions _eventProcessorOptions;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Microsoft.Azure.EventHubs.Processor.EventProcessorHostWrapper"/> class.
        /// </summary>
        /// <param name="eventHubConnectionString">Format: Endpoint=xxx.servicebus.windows.net/;SharedAccessKeyName=xxx;SharedAccessKey=xxx;EntityPath=xxx</param>
        /// <param name="storageConnectionString">Storage connection string for check pointing.</param>
        /// <param name="leaseContainerName">Lease container name  for check pointing.</param>
        /// <param name="getEventProcessor">A <see cref="Func{T}"/> to get a <see cref="IEventProcessor"/> instance.</param>
        /// <param name="consumerGroupName">Consumer group name. Defaults to '$Default'.</param>
        /// <param name="hostName">If value is null, a new Guid is used.</param>
        /// <param name="storageBlobPrefix">Storage BLOB prefix. Defaults to 'null'.</param>
        /// <param name="partitionManagerLeaseDurationInSeconds">see EventProcessorHost.PartitionManagerOptions.LeaseDuration, defaults to '30'.</param>
        /// <param name="partitionManagerRenewIntervalInSeconds">see EventProcessorHost.PartitionManagerOptions.RenewInterval, defaults to '10'.</param>
        /// <param name="maxBatchSize">see EventProcessorOptions.MaxBatchSize, defaults to '10'.</param>
        /// <param name="prefetchCount">see EventProcessorOptions.PrefetchCount, defaults to '300'.</param>
        /// <param name="receiveTimeoutInSeconds">see EventProcessorOptions.ReceiveTimeout, defaults to '60'.</param>
        /// <param name="invokeProcessorAfterReceiveTimeout">see EventProcessorOptions.InvokeProcessorAfterReceiveTimeout, defaults to 'false'.</param>
        public EventProcessorHostWrapper(
            string eventHubConnectionString,
            string storageConnectionString,
            string leaseContainerName,
            Func<IEventProcessor> getEventProcessor,
            string consumerGroupName = "$Default",
            string hostName = null,
            string storageBlobPrefix = null,
            int partitionManagerLeaseDurationInSeconds = 30,
            int partitionManagerRenewIntervalInSeconds = 10,
            int maxBatchSize = 10,
            int prefetchCount = 300,
            int receiveTimeoutInSeconds = 60,
            bool invokeProcessorAfterReceiveTimeout = false
        )
        {
            _eventProcessorHost
            = new EventProcessorHost(
            hostName ?? Guid.NewGuid().ToString(),
            default(string), //eventHubPath,
            consumerGroupName,
            eventHubConnectionString,
            storageConnectionString,
            leaseContainerName,
            storageBlobPrefix)
            {
                PartitionManagerOptions = new PartitionManagerOptions
                {
                    LeaseDuration = TimeSpan.FromSeconds(partitionManagerLeaseDurationInSeconds),
                    RenewInterval = TimeSpan.FromSeconds(partitionManagerRenewIntervalInSeconds)
                }
            };

            _eventProcessorFactory = new EventProcessorFactory(getEventProcessor);

            _eventProcessorOptions = new EventProcessorOptions
            {
                MaxBatchSize = maxBatchSize,
                PrefetchCount = prefetchCount,
                ReceiveTimeout = TimeSpan.FromSeconds(receiveTimeoutInSeconds),
                InvokeProcessorAfterReceiveTimeout = invokeProcessorAfterReceiveTimeout
            };
        }

        public Task RegisterEventProcessorFactoryAsync() => _eventProcessorHost.RegisterEventProcessorFactoryAsync(_eventProcessorFactory, _eventProcessorOptions);

        public Task UnregisterEventProcessorAsync() => _eventProcessorHost.UnregisterEventProcessorAsync();

        class EventProcessorFactory : IEventProcessorFactory
        {
            readonly Func<IEventProcessor> _Func;

            public EventProcessorFactory(Func<IEventProcessor> func)
            {
                _Func = func;
            }

            IEventProcessor IEventProcessorFactory.CreateEventProcessor(PartitionContext context) => _Func();
        }
    }
}