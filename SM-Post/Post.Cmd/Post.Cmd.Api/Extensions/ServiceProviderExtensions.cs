using CQRS.Core.Infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Infrastructure.CommandDispatcher;

namespace Post.Cmd.Api.Extensions;

public static class ServiceProviderExtensions
{
    public static void RegisterCommandHandlers(this IServiceProvider serviceProvider, CommandDispatcher commandDispatcher)
    {
        var commandHandler = serviceProvider.GetService<ICommandHandler>();

        var dispatcherType = commandDispatcher.GetType();
        var registerMethod = dispatcherType.GetMethod(nameof(ICommandDispatcher.RegisterHandler));
        var handlers = commandHandler.GetType().GetMethods().Where(m => m.Name.Equals(nameof(ICommandHandler.HandleAsync)));

        foreach (var handler in handlers)
        {
            var parameter = handler.GetParameters().First();

            var closedMethod = registerMethod.MakeGenericMethod(new Type[] { parameter.ParameterType });

            var funcType = closedMethod.GetParameters().FirstOrDefault();
            var handle = Delegate.CreateDelegate(funcType.ParameterType, commandHandler, handler.Name);

            closedMethod.Invoke(commandDispatcher, new object[] { handle });
        }
    }
}