using KooliProjekt.Data;

namespace KooliProjekt.Services
{
    public interface ICustomerService
    {
        Task<PagedResult<Customer>> List(int page, int pageSize);
        Task<Customer> Get(int id);
        Task Save(Customer list);
        Task Delete(int id);
    }
}