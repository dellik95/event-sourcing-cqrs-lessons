using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.EntityConfigurations;

public class CommentEntityConfiguration : IEntityTypeConfiguration<CommentEntity>
{
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        builder.HasKey(x => x.CommentId);
        builder.HasOne(x => x.Post).WithMany(x => x.PostComments).HasForeignKey(x => x.PostId);
    }
}