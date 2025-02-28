
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Data.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}