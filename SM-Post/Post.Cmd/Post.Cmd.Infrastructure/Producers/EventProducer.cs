using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;

namespace Post.Cmd.Infrastructure.Producers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _config;

    public EventProducer(ProducerConfig config)
    {
        _config = config;
    }
    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(this._config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();
        var message = new Message<string, string>()
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        var result = await producer.ProduceAsync(topic, message);
        if (result.Status == PersistenceStatus.NotPersisted)
        {
            /// TODO: Return result failure
            return;
        }
    }
}