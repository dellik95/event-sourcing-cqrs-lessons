using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface IPostRepository : IRepositoryBase<PostEntity>
{
    Task<List<PostEntity>> GetByAuthorAsync(string authorName);

    Task<List<PostEntity>> GetByLikesAsync(int likesCount);

    Task<List<PostEntity>> GetWithCommentsAsync();
}