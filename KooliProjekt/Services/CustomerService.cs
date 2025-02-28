using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _uof;

        public CustomerService(IUnitOfWork uof)
        {
            _uof = uof;
        }

        public async Task Delete(int Id)
        {
            await _uof.CustomerRepository.Delete(Id);
        }

        public async Task<Customer> Get(int Id)
        {
            return await _uof.CustomerRepository.Get((int)Id);
        }

        public async Task<PagedResult<Customer>> List(int page, int pageSize)
        {
            return await _uof.CustomerRepository.List(page, pageSize);
        }

        public async Task Save(Customer customer)
        {
            await _uof.CustomerRepository.Save(customer);
        }
    }
}