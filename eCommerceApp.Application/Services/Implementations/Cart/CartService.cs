using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Domain.Interfaces;
using eCommerceApp.Domain.Interfaces.Cart;

namespace eCommerceApp.Application.Services.Implementations.Cart
{
    public class CartService(ICart cartInterface, IMapper mapper, IGeneric<Product> productInterface, 
        IPaymentMethodService paymentMethodService, IPaymentService paymentService) : ICartService
    {
        public async Task<ServiceResponse> Checkout(Checkout checkout, string userId)
        {
            var (products, totalAmount) = await GetCartTotalAmount(checkout.Carts);
            var paymentMethods = await paymentMethodService.GetPaymentMethods();

            if (checkout.PaymentMethodId == paymentMethods.FirstOrDefault()!.Id)
                return await paymentService.Pay(totalAmount, products, checkout.Carts);
            else
                return new ServiceResponse(false, "Invalid Payment Method");
        }

        public async Task<ServiceResponse> SaveCheckoutHistory(IEnumerable<CreateArchive> archives)
        {
            var mappedData = mapper.Map<IEnumerable<Archive>>(archives);
            var result = await cartInterface.SaveCheckoutHistory(mappedData);
            return result > 0 ? new ServiceResponse(true, "Checkout Archived") : 
                new ServiceResponse(false, "Error occurred in saving");
        }

        private async Task<(IEnumerable<Product>, decimal)> GetCartTotalAmount(IEnumerable<ProcessCart> carts)
        {
            if (!carts.Any()) return ([], 0);

            var products = await productInterface.GetAllAsync();
            if (!products.Any()) return ([], 0);

            var cartProducts = carts
                .Select(cartItem => products.FirstOrDefault(p => p.Id == cartItem.ProductId))
                .Where(product => product != null)
                .ToList();

            var totalAmount = carts
                .Where(cartItem => cartProducts.Any(p => p.Id == cartItem.ProductId))
                .Sum(cartItem => cartItem.Quantity * cartProducts.First(p => p.Id == cartItem.ProductId)!.Price);
            return (cartProducts!, totalAmount);
        }
    }
}
