using KooliProjekt.Data;
using KooliProjekt.Search; // Make sure this is included

namespace KooliProjekt.Services
{
    public interface IInvoiceService
    {
        Task<PagedResult<Invoice>> List(int page, int pageSize, InvoiceSearch search = null);
        Task<List<Invoice>> Search(InvoiceSearch search);
        Task<Invoice> Get(int? Id);
        Task Save(Invoice invoice);
        Task Delete(int? Id);
        Task<bool> Includes(int Id);
    }
}

