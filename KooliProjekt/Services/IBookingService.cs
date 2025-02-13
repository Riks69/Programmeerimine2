using KooliProjekt.Data;

namespace KooliProjekt.Services
{
    public interface IBookingService
    {
        Task<PagedResult<Booking>> List(int page, int pageSize);
        Task<Booking> Get(int id);
        Task Save(Booking list);
        Task Delete(int id);
    }
}