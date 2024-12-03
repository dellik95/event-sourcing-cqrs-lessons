using Confluent.Kafka;
using CQRS.Core.Consumers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.BackgroundServices;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;
using EventHandler = Post.Query.Infrastructure.Handlers.EventHandler;

namespace Post.Query.Infrastructure.Extensions;

public static class DiExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager builderConfiguration)
    {
        var connectionString = builderConfiguration.GetConnectionString("PostConnection");
        services.AddDbContext<DatabaseContext>(
            x => x.UseSqlServer(connectionString)
                .UseLazyLoadingProxies(), ServiceLifetime.Transient);

        services.RegisterConfig<ConsumerConfig>(builderConfiguration.GetSection(nameof(ConsumerConfig)));
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IEventHandler, EventHandler>();
        services.AddScoped<IEventConsumer, EventConsumer>();
        services.AddHostedService<ConsumerHostedService>();


        return services;
    }
}