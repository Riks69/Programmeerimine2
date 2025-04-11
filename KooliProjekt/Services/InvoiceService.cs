using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KooliProjekt.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int? Id)
        {
            var invoice = await _context.Invoices.FindAsync(Id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Invoice> Get(int? Id)
        {
            return await _context.Invoices.FindAsync(Id);
        }

        public async Task<bool> Includes(int Id)
        {
            return await _context.Invoices.AnyAsync(c => c.Id == Id);
        }

        public async Task<PagedResult<Invoice>> List(int page, int pageSize, InvoiceSearch search = null)
        {
            var query = _context.Invoices.AsQueryable();

            // Kui otsingukriteerium on olemas ja Keyword pole tühi
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.BookingId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Amount.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Description.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.IsPaid.ToString(), $"%{search.Keyword}%"));
            }

            return await query.GetPagedAsync(page, pageSize);
        }


        // Uus Search meetod
        public async Task<List<Invoice>> Search(InvoiceSearch search)
        {
            IQueryable<Invoice> query = _context.Invoices;

            // Filtreeri Keyword järgi (see peaks olema esimene, et mitte mõjutada teisi filtreid)
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.BookingId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Amount.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Description.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.IsPaid.ToString(), $"%{search.Keyword}%"));
            }

            // Filtreeri Done (lõpetatud) järgi, kui see on määratud
            if (search?.Done.HasValue == true)
            {
                query = query.Where(h => h.IsPaid == search.Done);
            }

            return await query.ToListAsync(); // Tagastab kõik vastavad broneeringud
        }



        public async Task Save(Invoice invoice)
        {
            if (invoice.Id == 0)
            {
                // Kui ID on 0, siis lisame uue broneeringu
                _context.Invoices.Add(invoice);
            }
            else
            {
                // Kui ID on olemas, siis otsime broneeringut ID järgi
                var existingInvoice = await _context.Invoices.FindAsync(invoice.Id);

                if (existingInvoice == null)
                {
                    // Kui broneeringut ei leita, siis ei tee midagi
                    return;
                }

                // Kui broneering on olemas, siis uuendame andmeid
                existingInvoice.BookingId = invoice.BookingId;
                existingInvoice.Amount = invoice.Amount;
                existingInvoice.Description = invoice.Description;
                existingInvoice.IsPaid = invoice.IsPaid;
            }

            // Tagame, et muudatused salvestatakse ja ID määratakse õigesti
            await _context.SaveChangesAsync();
        }
    }
}
