using Microsoft.EntityFrameworkCore;
using Store.Domain.Products;

namespace Store.Data
{
    public class ApplicationDbContext : DbContext
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

       }   

       public DbSet<Category> Categories {get; set;}
    }
}