using CQRS.Core.Events;

namespace CQRS.Core.Infrastructure;

public interface IEventStore
{
    Task SaveEventsAsync(Guid aggregatorId, IEnumerable<BaseEvent> events, int expectedVersion);

    Task<IEnumerable<BaseEvent>> GetEventsAsync(Guid aggregatorId);
    Task<List<Guid>> GetAggregateIdsAsync();
}