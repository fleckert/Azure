# Microsoft.Azure.IotHub.Relay

This app provides an http endpoint to relay messages to an Azure IotHub.

During development, the need to ingest a telemetry message to trigger some specific backend event comes up quite frequently.

Take your favourite tool to do a http post request and you are good to go... create run books... go crazy...

```javascript
{
   "deviceId":"device0",
   "properties":{
      "key1":"value1",
      "key2":"value2"
   },
   "payload":"Hello World"
}
```


# Microsoft.Azure.EventHubs.Helper

convenience wrapper for Microsoft.Azure.EventHubs.Processor.EventProcessorHost
```csharp
var eventProcessorHostWrapper = new EventProcessorHostWrapper(
    eventHubConnectionString: "...",
    storageConnectionString: "...",
    leaseContainerName: "...",
    getEventProcessor: () => new SomeEventProcessor()
);

await eventProcessorHostWrapper.RegisterEventProcessorFactoryAsync();
...
..
.
await eventProcessorHostWrapper.UnregisterEventProcessorAsync();

class SomeEventProcessor : IEventProcessor {...}

```
