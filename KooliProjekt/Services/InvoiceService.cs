using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _uof;

        public InvoiceService(IUnitOfWork uof)
        {
            _uof = uof;
        }

        public async Task Delete(int Id)
        {
            await _uof.InvoiceRepository.Delete(Id);
        }

        public async Task<Invoice> Get(int Id)
        {
            return await _uof.InvoiceRepository.Get((int)Id);
        }

        public async Task<PagedResult<Invoice>> List(int page, int pageSize)
        {
            return await _uof.InvoiceRepository.List(page, pageSize);
        }

        public async Task Save(Invoice invoice)
        {
            await _uof.InvoiceRepository.Save(invoice);
        }
    }
}