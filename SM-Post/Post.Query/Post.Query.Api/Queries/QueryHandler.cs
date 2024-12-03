using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries;

public class QueryHandler : IQueryHandler
{

    private readonly IPostRepository _repository;

    public QueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query) =>
        await this._repository.GetAllAsync();

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query) =>
        (await _repository.GetByIdAsync(query.PostId)).AsSequence();

    public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query) =>
        await this._repository.GetByAuthorAsync(query.AuthorName);

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query) =>
        await this._repository.GetWithCommentsAsync();

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query) =>
        await this._repository.GetByLikesAsync(query.NumberOfLikes);

}