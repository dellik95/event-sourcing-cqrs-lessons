using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services.BuildServiceProvider();

var handler = services.GetRequiredService<IQueryHandler>();
var dispatcher = new QueryDispatcher();

dispatcher.RegisterHandler<FindAllPostsQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindPostByIdQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindPostsByAuthorQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithCommentsQuery>(handler.HandleAsync);
dispatcher.RegisterHandler<FindPostsWithLikesQuery>(handler.HandleAsync);

builder.Services.AddSingleton<IQueryDispatcher<PostEntity>>(_ => dispatcher);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
