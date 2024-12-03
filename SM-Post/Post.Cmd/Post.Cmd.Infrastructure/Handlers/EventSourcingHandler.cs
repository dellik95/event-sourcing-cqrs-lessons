using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;
using CQRS.Core.Producers;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }
    public async Task SaveAsync(AggregateRoot aggregateRoot)
    {
        await this._eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(),
            aggregateRoot.Version);
        aggregateRoot.MarkChangesIsCommitted();
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();

        var events = await this._eventStore.GetEventsAsync(aggregateId);
        if (events == null || !events.Any())
        {
            ///TODO return failure result
            return aggregate;
        }

        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();

        return aggregate;
    }

    public async Task ReplayAllEvents()
    {
        var ids = await this._eventStore.GetAggregateIdsAsync();

        foreach (var id in ids)
        {
            var aggregator = new PostAggregate();

            var events = await this._eventStore.GetEventsAsync(id);
            var orderedEvents = events.OrderBy(x => x.Version).ToList();

            aggregator.ReplayEvents(orderedEvents);

            if (!aggregator.Active)
            {
                continue;
            }

            ///TODO: Get from Config
            var topic = "KAFKA_TOPIC";
            foreach (var @event in orderedEvents)
            {
                await this._eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}