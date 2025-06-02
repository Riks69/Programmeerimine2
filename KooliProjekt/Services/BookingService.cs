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

            // Kui otsingukriteerium on olemas ja Keyword pole tühi
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

            // Filtreeri Keyword järgi (see peaks olema esimene, et mitte mõjutada teisi filtreid)
            if (search != null && !string.IsNullOrEmpty(search.Keyword))
            {
                query = query.Where(h =>
                    EF.Functions.Like(h.UserId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.CarId.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.EndTime.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.DistanceKm.ToString(), $"%{search.Keyword}%") ||
                    EF.Functions.Like(h.StartTime.ToString(), $"%{search.Keyword}%"));
            }

            // Filtreeri Done (lõpetatud) järgi, kui see on määratud
            if (search?.Done.HasValue == true)
            {
                query = query.Where(h => h.IsCompleted == search.Done);
            }

            return await query.ToListAsync(); // Tagastab kõik vastavad broneeringud
        }



        public async Task Save(Booking booking)
        {
            if (booking.Id == 0)
            {
                // Kui ID on 0, siis lisame uue broneeringu
                _context.Bookings.Add(booking);
            }
            else
            {
                // Kui ID on olemas, siis otsime broneeringut ID järgi
                var existingBooking = await _context.Bookings.FindAsync(booking.Id);

                if (existingBooking == null)
                {
                    // Kui broneeringut ei leita, siis ei tee midagi
                    return;
                }

                // Kui broneering on olemas, siis uuendame andmeid
                existingBooking.CarId = booking.CarId;
                existingBooking.DistanceKm = booking.DistanceKm;
                existingBooking.EndTime = booking.EndTime;
                existingBooking.IsCompleted = booking.IsCompleted;
                existingBooking.StartTime = booking.StartTime;
                existingBooking.UserId = booking.UserId;
            }

            // Tagame, et muudatused salvestatakse ja ID määratakse õigesti
            await _context.SaveChangesAsync();
        }
    }
}
