using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Data.Identity;
using Store.Domain.Products;
using Store.Domain.Sales;

namespace Store.Data.Contexties
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

       }   

       public DbSet<Category> Categories {get; set;}
       public DbSet<Product> Products { get; set; }

       public DbSet<Sale> Sales { get; set; }
    }
}