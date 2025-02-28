using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _uof;

        public BookingService(IUnitOfWork uof)
        {
            _uof = uof;
        }

        public async Task Delete(int Id)
        {
            await _uof.BookingRepository.Delete(Id);
        }

        public async Task<Booking> Get(int Id)
        {
            return await _uof.BookingRepository.Get((int)Id);
        }

        public async Task<PagedResult<Booking>> List(int page, int pageSize)
        {
            return await _uof.BookingRepository.List(page, pageSize);
        }

        public async Task Save(Booking booking)
        {
            await _uof.BookingRepository.Save(booking);
        }
    }
}