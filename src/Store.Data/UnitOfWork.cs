using System.Threading.Tasks;
using Store.Data.Contexties;
using Store.Domain;

namespace Store.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context) {
            _context = context;
        }

        public async Task Commit() {
            await _context.SaveChangesAsync();
        }
    }
}