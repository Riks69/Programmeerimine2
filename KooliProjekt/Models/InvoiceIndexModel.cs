using KooliProjekt.Data;
using KooliProjekt.Search;

namespace KooliProjekt.Models
{
    public class InvoiceIndexModel

    {
        public InvoiceSearch Search { get; set; }
        public PagedResult<Invoice> Data { get; set; }
    }
}
