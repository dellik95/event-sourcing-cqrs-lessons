using CQRS.Core.Domain;
using CQRS.Core.Events;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public EventStoreRepository(MongoDbConfig config)
    {
        var mongoClient = new MongoClient(config.ConnectionString);

        var dataBase = mongoClient.GetDatabase(config.DatabaseName);

        _eventStoreCollection = dataBase.GetCollection<EventModel>(config.CollectionName);
    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }

    public async Task<IEnumerable<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<EventModel>> GetAllAsync() =>
        await _eventStoreCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
}