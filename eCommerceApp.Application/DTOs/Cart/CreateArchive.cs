using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Application.DTOs.Cart
{
    public class CreateArchive
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public Guid UserId { get; set; }
    }
}
