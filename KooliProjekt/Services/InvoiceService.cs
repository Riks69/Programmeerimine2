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

            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.BookingId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Amount.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.Description.ToString(), $"%{search.Keyword}%"));


            }

            return await query.GetPagedAsync(page, pageSize);
        }



        public async Task Save(Invoice invoice)
        {
            if (invoice.Id == 0)
            {
                _context.Invoices.Add(invoice);  // Kui ID on 0, siis lisame uue arve
            }
            else
            {
                var existingInvoice = await _context.Invoices.FindAsync(invoice.Id); // Otsime olemasolevat arvet
                if (existingInvoice != null)
                {
                    // Kui arve on olemas, siis uuendame selle
                    existingInvoice.BookingId = invoice.BookingId;
                    existingInvoice.Amount = invoice.Amount;
                    existingInvoice.Description = invoice.Description;

                    // Ei ole vaja eraldi Entry muudatusi teha, kuna FindAsync tagastab juba jälgimisobjekti
                }
                else
                {
                    // Kui arvet ei leita, lisame selle uue
                    _context.Invoices.Add(invoice);
                }
            }

            await _context.SaveChangesAsync();  // Salvesta muudatused andmebaasi
        }

    }
}

