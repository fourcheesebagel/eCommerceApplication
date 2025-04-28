using System.ComponentModel.DataAnnotations;

namespace eCommerceApp.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Product> Products { get; set; } //establishes relationship with Product one category to many products

    }
    //CTRL+. -> moves class to a new file
}
