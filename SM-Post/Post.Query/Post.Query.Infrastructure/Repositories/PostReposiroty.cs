using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository<PostEntity>, IPostRepository
    {
        public PostRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<List<PostEntity>> GetByAuthorAsync(string authorName)
        {
            return await this.GetBy(x => x.Author == authorName).Include(x => x.PostComments).ToListAsync();
        }

        public async Task<List<PostEntity>> GetByLikesAsync(int likesCount)
        {
            return await this.GetBy(x => x.Likes >= likesCount).Include(x => x.PostComments).ToListAsync();
        }

        public async Task<List<PostEntity>> GetWithCommentsAsync()
        {
            return await this.GetBy(x => x.PostComments != null && x.PostComments.Any()).Include(x => x.PostComments).ToListAsync();
        }
    }
}
