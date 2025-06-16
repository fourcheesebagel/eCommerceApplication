using eCommerceApp.Domain.Entities;
using eCommerceApp.Domain.Entities.Cart;
using eCommerceApp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eCommerceApp.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<Archive> CheckoutArchives { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PaymentMethod>()
                .HasData(
                new PaymentMethod
                {
                    Id = Guid.NewGuid(),
                    Name = "Credit Card",
                });

            builder.Entity<IdentityRole>()
                .HasData(
                new IdentityRole
                {
                    Id = "1217ce24-eb1c-4a4d-b6da-1ef8916afb70", //Hardcoded
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "d633c330-f47c-41a3-966b-7f7c7239d1df",
                    Name = "User",
                    NormalizedName = "USER"
                });
        }
    }
}
