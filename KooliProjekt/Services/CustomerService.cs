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

            // Kui otsingukriteerium on olemas ja Keyword pole tühi
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.Name.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Password.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Email.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.IsRegistered.ToString(), $"%{search.Keyword}%"));
            }

            return await query.GetPagedAsync(page, pageSize);
        }


        // Uus Search meetod
        public async Task<List<Customer>> Search(CustomerSearch search)
        {
            IQueryable<Customer> query = _context.Customers;

            // Filtreeri Keyword järgi (see peaks olema esimene, et mitte mõjutada teisi filtreid)
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.Name.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Password.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Email.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.IsRegistered.ToString(), $"%{search.Keyword}%"));
            }

            // Filtreeri Done (lõpetatud) järgi, kui see on määratud
            if (search?.Done.HasValue == true)
            {
                query = query.Where(h => h.IsRegistered == search.Done);
            }

            return await query.ToListAsync(); // Tagastab kõik vastavad broneeringud
        }



        public async Task Save(Customer customer)
        {
            if (customer.Id == 0)
            {
                // Kui ID on 0, siis lisame uue broneeringu
                _context.Customers.Add(customer);
            }
            else
            {
                // Kui ID on olemas, siis otsime broneeringut ID järgi
                var existingCustomer = await _context.Customers.FindAsync(customer.Id);

                if (existingCustomer == null)
                {
                    // Kui broneeringut ei leita, siis ei tee midagi
                    return;
                }

                // Kui broneering on olemas, siis uuendame andmeid
                existingCustomer.Name = customer.Name;
                existingCustomer.Password = customer.Password;
                existingCustomer.Email = customer.Email;
                existingCustomer.IsRegistered = customer.IsRegistered;
            }

            // Tagame, et muudatused salvestatakse ja ID määratakse õigesti
            await _context.SaveChangesAsync();
        }
    }
}
