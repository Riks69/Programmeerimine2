using System.Collections.Generic;
using System.Threading.Tasks;

namespace KooliProjekt.PublicAPI.Api
{
    public interface IApiClient
    {
        Task<Result<List<Customer>>> List();
        Task<Result<Customer>> Get(int id);
        Task<Result> Save(Customer customer);
        Task<Result> Delete(int id);
    }
}
