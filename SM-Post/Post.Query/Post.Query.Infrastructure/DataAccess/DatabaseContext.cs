using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Infrastructure.EntityConfigurations;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContext : DbContext
{
    public DbSet<PostEntity> Posts { get; set; }

    public DbSet<CommentEntity> Comments { get; set; }


    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PostEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CommentEntityConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}