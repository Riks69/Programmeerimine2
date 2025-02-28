
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Data.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}