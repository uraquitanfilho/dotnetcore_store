using System.Threading.Tasks;

namespace Store.Domain
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}