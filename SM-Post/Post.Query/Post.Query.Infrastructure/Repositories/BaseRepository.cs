using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class BaseRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        private readonly DatabaseContext _context;
        protected readonly DbSet<TEntity> DataSet;
        public BaseRepository(DatabaseContext context)
        {
            _context = context;
            DataSet = context.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            try
            {
                this.DataSet.Add(entity);
                await this._context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public async Task UpdateAsync(TEntity entity)
        {
            this.DataSet.Update(entity);
            await this._context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                this.DataSet.Remove(entity);
            }
            await this._context.SaveChangesAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await this.DataSet.FindAsync(id);
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await this.GetBy().ToListAsync();
        }

        protected IQueryable<TEntity> GetBy(Expression<Func<TEntity, bool>> predicate = null, bool isNoTracking = false)
        {
            IQueryable<TEntity> query = this.DataSet.AsQueryable();
            if (predicate != null)
            {
                query = this.DataSet.Where(predicate);
            }

            if (isNoTracking)
            {
                query = this.DataSet.AsNoTracking();
            }

            return query;
        }
    }
}
