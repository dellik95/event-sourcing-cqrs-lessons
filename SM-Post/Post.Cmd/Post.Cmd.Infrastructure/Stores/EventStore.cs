using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;

    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
    }

    public async Task SaveEventsAsync(Guid aggregatorId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventsStream = (await this._eventStoreRepository.FindByAggregateId(aggregatorId)).ToList();
        if (expectedVersion != -1 && eventsStream[^1].Version != expectedVersion)
        {
            /// TODO Add failure result
            return;
        }

        var version = expectedVersion;
        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;
            var eventModel = new EventModel()
            {
                AggregateIdentifier = aggregatorId,
                TimeStamp = DateTime.Now,
                Version = version,
                EventData = @event,
                AggregateType = nameof(PostAggregate),
                EventType = eventType
            };

            await this._eventStoreRepository.SaveAsync(eventModel);

            ///TODO: Get from Config
            var topic = "KAFKA_TOPIC";

            await this._eventProducer.ProduceAsync(topic, @event);
        }
    }

    public async Task<IEnumerable<BaseEvent>> GetEventsAsync(Guid aggregatorId)
    {
        var events = await this._eventStoreRepository.FindByAggregateId(aggregatorId);
        if (events == null || !events.Any())
        {
            /// TODO Add failure result
            return null;
        }

        return events.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }

    public async Task<List<Guid>> GetAggregateIdsAsync()
    {
        var events = await this._eventStoreRepository.GetAllAsync();
        if (events == null || !events.Any())
        {
            return new List<Guid>();
        }

        var aggregateIds = events.AsParallel().Select(x => x.AggregateIdentifier).Distinct().ToList();

        return aggregateIds;
    }
}