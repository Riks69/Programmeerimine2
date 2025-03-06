using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Booking>> List(int page, int pageSize, BookingSearch search = null)
        {
            var query = _context.Bookings.AsQueryable();

            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.UserId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.CarId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.EndTime.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.DistanceKm.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.StartTime.ToString(), $"%{search.Keyword}%"));

            }

            return await query.GetPagedAsync(page, pageSize);
        }

        public async Task<Booking> Get(int id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task Save(Booking list)
        {
            if (list.Id == 0)
            {
                _context.Add(list);
            }
            else
            {
                _context.Update(list);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }
    }
}
