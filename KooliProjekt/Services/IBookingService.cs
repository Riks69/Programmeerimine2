using KooliProjekt.Data;
using KooliProjekt.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KooliProjekt.Services
{
    public interface IBookingService
    {
        Task<Booking> Get(int? Id);
        Task Save(Booking booking);
        Task Delete(int? Id);
        Task<bool> Includes(int Id);
        Task<PagedResult<Booking>> List(int page, int pageSize, BookingSearch search = null);
        Task<List<Booking>> Search(BookingSearch search);
    }
}
