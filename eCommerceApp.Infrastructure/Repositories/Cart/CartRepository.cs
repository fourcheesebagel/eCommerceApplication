using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Domain.Interfaces.Cart;
using eCommerceApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Repositories.Cart
{
    public class CartRepository(AppDbContext context) : ICart
    {
        public async Task<IEnumerable<Archive>> GetAllCheckoutHistory()
        {
            return await context.CheckoutArchives.AsNoTracking().ToListAsync();
        }

        public async Task<int> SaveCheckoutHistory(IEnumerable<Archive> checkouts)
        {
            context.CheckoutArchives.AddRange(checkouts);
            return await context.SaveChangesAsync();
        }
    }
}
