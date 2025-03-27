using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KooliProjekt.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int? Id)
        {
            var booking = await _context.Bookings.FindAsync(Id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Booking> Get(int? Id)
        {
            return await _context.Bookings.FindAsync(Id);
        }

        public async Task<bool> Includes(int Id)
        {
            return await _context.Bookings.AnyAsync(c => c.Id == Id);
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

        // Uus Search meetod
        public async Task<List<Booking>> Search(BookingSearch search)
        {
            IQueryable<Booking> query = _context.Bookings;

            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.UserId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.CarId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.EndTime.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.DistanceKm.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.StartTime.ToString(), $"%{search.Keyword}%"));
            }

            return await query.ToListAsync(); // Tagastab kõik vastavad broneeringud
        }

        public async Task Save(Booking booking)
        {
            if (booking.Id == 0)
            {
                _context.Bookings.Add(booking);
            }
            else
            {
                var existingBookings = await _context.Bookings.FindAsync(booking.Id);

                if (existingBookings != null)
                {
                    // If it exists, update the entity
                    _context.Entry(existingBookings).State = EntityState.Modified;
                }
                else
                {
                    _context.Bookings.Add(booking);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
