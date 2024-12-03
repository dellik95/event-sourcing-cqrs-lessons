using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher<PostEntity>
    {
        private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _handlers = new();
        public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
        {
            var type = typeof(TQuery);
            if (this._handlers.ContainsKey(type))
            {
                throw new ArgumentException($"Handler for {type.Name} have already exist");
            }
            this._handlers.Add(type, x => handler((TQuery)x));
        }

        public Task<List<PostEntity>> SendAsync(BaseQuery query)
        {
            var type = query.GetType();
            if (this._handlers.TryGetValue(type, out var handler))
            {
                return handler(query);
            }

            throw new ArgumentOutOfRangeException($"Handler for {type.Name} doesn`t exist");
        }
    }
}
