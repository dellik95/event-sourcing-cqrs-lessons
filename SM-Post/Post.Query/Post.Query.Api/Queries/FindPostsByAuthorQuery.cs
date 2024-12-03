using CQRS.Core.Queries;

namespace Post.Query.Api.Queries;

public class FindPostsByAuthorQuery : BaseQuery
{
    public string AuthorName { get; set; }
}