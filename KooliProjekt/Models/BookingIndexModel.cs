using KooliProjekt.Data;
using KooliProjekt.Search;

namespace KooliProjekt.Models
{
    public class BookingIndexModel

    {
        public BookingSearch Search { get; set; }
        public PagedResult<Booking> Data { get; set; }
    }
}
