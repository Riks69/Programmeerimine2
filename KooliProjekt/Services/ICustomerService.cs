using KooliProjekt.Data;
using KooliProjekt.Search;

namespace KooliProjekt.Services
{
    public interface ICustomerService
    {
        Task<PagedResult<Customer>> List(int page, int pageSize, CustomerSearch search);
        Task<Customer> Get(int id);
        Task Save(Customer list);
        Task Delete(int id);
    }
}