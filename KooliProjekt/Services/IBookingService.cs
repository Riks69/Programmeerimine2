using KooliProjekt.Data;
using KooliProjekt.Search;

namespace KooliProjekt.Services
{
    public interface IBookingService
    {
        Task<PagedResult<Booking>> List(int page, int pageSize, BookingSearch search);
        Task<Booking> Get(int id);
        Task Save(Booking list);
        Task Delete(int id);
    }
}