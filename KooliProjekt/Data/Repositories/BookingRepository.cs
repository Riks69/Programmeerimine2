
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Data.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}