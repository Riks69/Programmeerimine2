using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KooliProjekt.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int? Id)
        {
            var customer = await _context.Customers.FindAsync(Id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Customer> Get(int? Id)
        {
            return await _context.Customers.FindAsync(Id);
        }

        public async Task<bool> Includes(int Id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == Id);
        }

        public async Task<PagedResult<Customer>> List(int page, int pageSize, CustomerSearch search = null)
        {
            var query = _context.Customers.AsQueryable();

            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.Name.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Password.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Email.ToString(), $"%{search.Keyword}%"));
            }

            return await query.GetPagedAsync(page, pageSize);
        }



        public async Task Save(Customer customer)
        {
            if (customer.Id == 0)
            {
                _context.Customers.Add(customer);
            }
            else
            {
                var existingCustomers = await _context.Customers.FindAsync(customer.Id);

                if (existingCustomers != null)
                {
                    // If it exists, update the entity
                    _context.Entry(existingCustomers).State = EntityState.Modified;
                }
                else
                {
                    _context.Customers.Add(customer);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}

