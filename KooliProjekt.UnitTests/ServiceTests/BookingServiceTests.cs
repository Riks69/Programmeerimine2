using KooliProjekt.Data;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KooliProjekt.Services;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class BookingServiceTests
    {
        private readonly BookingService _bookingService;
        private readonly ApplicationDbContext _context;

        public BookingServiceTests()
        {
            // Kasutame InMemory andmebaasi, et simuleerida andmebaasi käitumist
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _bookingService = new BookingService(_context);
        }

        [Fact]
        public async Task Delete_ShouldRemoveBooking_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { Id = 1, UserId = 1 };
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Act
            await _bookingService.Delete(booking.Id);

            // Assert
            var deletedBooking = await _context.Bookings.FindAsync(booking.Id);
            Assert.Null(deletedBooking); // Broneeringut ei tohiks enam olla
        }

        [Fact]
        public async Task Get_ShouldReturnBooking_WhenBookingExists()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, UserId = 1 };
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingService.Get(bookingId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookingId, result.Id);
        }

        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenBookingExists()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, UserId = 1 };
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingService.Includes(bookingId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task List_ShouldReturnPagedResult_WhenSearchIsProvided()
        {
            // Arrange
            var search = new BookingSearch { Keyword = "test" }; // Otsingutermin
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, UserId = 1, CarId = 1, DistanceKm = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
                new Booking { Id = 2, UserId = 2, CarId = 2, DistanceKm = 200, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1) },
                new Booking { Id = 3, UserId = 3, CarId = 3, DistanceKm = 300, StartTime = DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddHours(1) }
            };

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingService.List(1, 2, search); // 1. leht, 2 kirjet lehe kohta

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count); // Kontrollime, et tagastataks ainult 2 broneeringut (pagineerimine)
            Assert.True(result.Results.All(b => b.UserId.ToString().Contains(search.Keyword) ||
                                                 b.CarId.ToString().Contains(search.Keyword) ||
                                                 b.StartTime.ToString().Contains(search.Keyword) ||
                                                 b.EndTime.ToString().Contains(search.Keyword) ||
                                                 b.DistanceKm.ToString().Contains(search.Keyword))); // Kontrollime, et otsingutermin sisaldub vähemalt ühes väljas
        }

        [Fact]
        public async Task List_ShouldReturnPagedResult_WhenNoSearchIsProvided()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, UserId = 1, CarId = 1, DistanceKm = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
                new Booking { Id = 2, UserId = 2, CarId = 2, DistanceKm = 200, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1) },
                new Booking { Id = 3, UserId = 3, CarId = 3, DistanceKm = 300, StartTime = DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddHours(1) }
            };

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingService.List(1, 2); // 1. leht, 2 kirjet lehe kohta

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count); // Kontrollime, et tagastataks ainult 2 broneeringut (pagineerimine)
        }

        [Fact]
        public async Task Save_ShouldAddBooking_WhenBookingIsNew()
        {
            // Arrange
            var newBooking = new Booking { Id = 0, UserId = 1 };

            // Act
            await _bookingService.Save(newBooking);

            // Assert
            var savedBooking = await _context.Bookings.FindAsync(newBooking.Id);
            Assert.NotNull(savedBooking);
            Assert.Equal(newBooking.UserId, savedBooking.UserId);
        }

        [Fact]
        public async Task Save_ShouldUpdateBooking_WhenBookingExists()
        {
            // Arrange
            var existingBooking = new Booking { Id = 1, UserId = 1 };
            await _context.Bookings.AddAsync(existingBooking);
            await _context.SaveChangesAsync();

            // Muutame olemasolevat broneeringut
            existingBooking.UserId = 2;

            // Tagame, et EF Core käsitleb muudatust õigesti
            _context.Entry(existingBooking).State = EntityState.Modified;

            // Act
            await _bookingService.Save(existingBooking);

            // Assert
            var updatedBooking = await _context.Bookings.FindAsync(existingBooking.Id);
            Assert.NotNull(updatedBooking);
            Assert.Equal(2, updatedBooking.UserId);
        }
    }
}
