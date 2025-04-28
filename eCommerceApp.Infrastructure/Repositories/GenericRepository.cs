using eCommerceApp.Domain.Interfaces;
namespace eCommerceApp.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGeneric<TEntity> where TEntity : class
    {
        public Task<int> AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
