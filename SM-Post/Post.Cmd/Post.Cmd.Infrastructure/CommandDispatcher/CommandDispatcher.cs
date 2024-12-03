using System.Collections.Concurrent;
using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.CommandDispatcher
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ConcurrentDictionary<Type, List<Func<BaseCommand, Task>>> _handlers = new();

        public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
        {
            Task ModifiedHandler(BaseCommand x) => handler((T)x);
            this._handlers.AddOrUpdate(typeof(T), type => new List<Func<BaseCommand, Task>>()
                {
                    ModifiedHandler
                },
                (type, list) =>
                {
                    list.Add(ModifiedHandler);
                    return list;
                });
        }

        public async Task SendAsync(BaseCommand command)
        {
            var commandType = command.GetType();
            if (this._handlers.TryGetValue(commandType, out var commandHandlers))
            {
                var tasks = commandHandlers.Select(x => x(command)).ToList();
                await Task.WhenAll(tasks);
                return;
            }

            throw new ArgumentOutOfRangeException($"Handlers for {commandType} not found!");
        }
    }
}
