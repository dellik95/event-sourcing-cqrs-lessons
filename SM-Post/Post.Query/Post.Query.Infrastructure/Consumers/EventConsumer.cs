using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _consumerConfig;

    private readonly IEventHandler _handler;
    public EventConsumer(ConsumerConfig consumerConfig, IEventHandler handler)
    {
        this._consumerConfig = consumerConfig;
        _handler = handler;
    }

    public async Task Consume(string topic)
    {
        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new EventJsonConverter()
            }
        };

        using var consumer = new ConsumerBuilder<string, string>(this._consumerConfig)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);
        while (true)
        {
            try
            {
                var consumeResult = consumer.Consume();
                if (consumeResult?.Message == null)
                {
                    continue;
                }

                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
                await _handler.On(@event.GetType(), @event);

                consumer.Commit(consumeResult);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}