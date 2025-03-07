using KooliProjekt.Data;
using KooliProjekt.Search; // Make sure this is included

namespace KooliProjekt.Services
{
    public interface IBookingService
    {
        Task<PagedResult<Booking>> List(int page, int pageSize, BookingSearch search = null);
        Task<Booking> Get(int? Id);
        Task Save(Booking booking);
        Task Delete(int? Id);
        Task<bool> Includes(int Id);
    }
}