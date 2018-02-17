using System.Collections.Generic;
using System.Linq;
using Store.Data.Contexties;
using Store.Domain;

namespace Store.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity: Entity
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context) {
            _context = context;
        }

        public virtual IEnumerable<TEntity> All() {
            return _context.Set<TEntity>().AsEnumerable();
        }

        public virtual TEntity GetById(int id) {
           var query = _context.Set<TEntity>().Where(e => e.Id == id);
           if(query.Any())
             return query.First();
           return null;  
        }

        public virtual void Save(TEntity entity) {
           _context.Set<TEntity>().Add(entity);
        }
    }
}