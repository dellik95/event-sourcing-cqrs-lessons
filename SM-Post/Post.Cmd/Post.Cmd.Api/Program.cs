using CQRS.Core.Infrastructure;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.Extensions;
using Post.Cmd.Infrastructure.CommandDispatcher;
using Post.Cmd.Infrastructure.Extensions;
using System.Reflection;
using CQRS.Core.Events;
using MongoDB.Bson.Serialization;
using Post.Common;

static void RegisterBsonClassesForBaseType<TBase>()
{
    var baseType = typeof(TBase);
    var exportedTypes = typeof(AssemblyReference).Assembly.GetExportedTypes();
    var derivedTypes = exportedTypes.Where(t => t.IsSubclassOf(baseType)).ToList();
    var registerMethod = typeof(BsonClassMap).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m => m.Name.Equals(nameof(BsonClassMap.RegisterClassMap)) && m.IsGenericMethod);
    foreach (var derivedType in derivedTypes)
    {
        var closedMethod = registerMethod?.MakeGenericMethod(new[] { derivedType });
        closedMethod?.Invoke(null, null);
    }
}

RegisterBsonClassesForBaseType<BaseEvent>();


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICommandHandler, CommandHandler>();



builder.Services.AddSingleton<ICommandDispatcher>(provider =>
{
    var dispatcher = new CommandDispatcher();
    using var scope = provider.CreateScope();
    scope.ServiceProvider.RegisterCommandHandlers(dispatcher);
    return dispatcher;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
