using eCommerceApp.Application.DTOs.Category;
namespace eCommerceApp.Application.DTOs.Product
{
    public class GetProduct : ProductBase
    {
        public Guid Id { get; set; }
        public GetCategory? Category { get; set; }
    }
}
