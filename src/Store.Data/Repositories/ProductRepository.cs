using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Store.Data.Contexties;
using Store.Domain.Products;

namespace Store.Data.Repositories
{
    public class ProductRepository : Repository<Product>
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override Product GetById(int id)
        {
            var query = _context.Set<Product>().Include(p => p.Category).Where(e => e.Id == id);
            if(query.Any())
                return query.First();
            return null;
        }

        public override IEnumerable<Product> All()
        {
            var query = _context.Set<Product>().Include(p => p.Category);

            return query.Any() ? query.ToList() : new List<Product>();
        }
    }
}