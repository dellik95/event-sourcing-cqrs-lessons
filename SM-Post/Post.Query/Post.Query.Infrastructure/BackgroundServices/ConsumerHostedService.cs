using Amazon.Runtime.Internal.Util;
using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Post.Query.Infrastructure.BackgroundServices;

public class ConsumerHostedService : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Event eventConsumer Service Started.");

        var topic = "KAFKA_TOPIC";


        Task.Run(async () =>
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetService<IEventConsumer>();
            await consumer.Consume(topic);
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Event eventConsumer Service Stopped.");
        return Task.CompletedTask;
    }
}