namespace eCommerceApp.Application.DTOs.Product
{
    public class CreateProduct : ProductBase // : between is an inheritance meaning it will take all the ProductBase properties into the class its being injected into
    {

    }

    public class CreateCategory : CategoryBase
    {
    }

    public class UpdateCategory : CategoryBase
    {
        public Guid Id { get; set; }
    }

    public class CategoryBase
    {
        public string? Name { get; set; }
    }
}
