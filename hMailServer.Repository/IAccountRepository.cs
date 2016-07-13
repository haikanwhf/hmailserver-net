using System.Threading.Tasks;
using hMailServer.Entities;

namespace hMailServer.Repository
{
    public interface IAccountRepository
    {
       Task<Account> GetByNameAsync(string address);
    }
}
