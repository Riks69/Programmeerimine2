using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<Customer> Get(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task Save(Customer list)
        {
            if (list.Id == 0)
            {
                _context.Add(list);
            }
            else
            {
                _context.Update(list);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
