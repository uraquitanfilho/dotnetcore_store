using System.Threading.Tasks;

namespace Store.Domain.Account
{
    public interface IAuthentication
    {
         Task<bool> Authenticate(string email, string password);
    }
}