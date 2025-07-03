using eCommerceApp.Application.DTOs.Cart;
using eCommerceApp.Application.Services.Interfaces.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpPost("checkout")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Checkout(Checkout checkout)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = await cartService.Checkout(checkout);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("save-checkout")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SaveCheckout(IEnumerable<CreateArchive> archives)
        {
            var result = await cartService.SaveCheckoutHistory(archives);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get-archives")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCheckoutHistory()
        {
            var archives = await cartService.GetArchives();
            return archives.Any() ? Ok(archives) : NotFound();
        }
    }
}
