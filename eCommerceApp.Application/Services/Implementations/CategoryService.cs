using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.Services.Interfaces;
namespace eCommerceApp.Application.Services.Implementations
{
    internal class CategoryService : ICategoryService
    {
        public Task<ServiceResponse> AddAsync(CreateCategory category)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetCategory>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GetCategory> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse> UpdateAsync(UpdateCategory category)
        {
            throw new NotImplementedException();
        }
    }
}
