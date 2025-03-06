using KooliProjekt.Data;
using KooliProjekt.Search;

namespace KooliProjekt.Services
{
    public interface IInvoiceService
    {
        Task<PagedResult<Invoice>> List(int page, int pageSize, InvoiceSearch search);
        Task<Invoice> Get(int id);
        Task Save(Invoice list);
        Task Delete(int id);
    }
}