namespace Post.Query.Domain.Entities;

public class PostEntity
{
    public Guid Id { get; set; }

    public string Author { get; set; }

    public DateTime DatePosted { get; set; }

    public string Message { get; set; }

    public int Likes { get; set; }

    public virtual ICollection<CommentEntity> PostComments { get; set; }
}