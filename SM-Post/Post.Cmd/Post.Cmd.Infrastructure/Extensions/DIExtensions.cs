using System.Reflection;
using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;
using Post.Common.Extensions;

namespace Post.Cmd.Infrastructure.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.RegisterConfig<MongoDbConfig>(configurationManager.GetSection(MongoDbConfig.Key));
            services.RegisterConfig<ProducerConfig>(configurationManager.GetSection(nameof(ProducerConfig)));
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<IEventStore, EventStore>();
            services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
            services.AddScoped<IEventProducer, EventProducer>();
            return services;
        }
    }
}
