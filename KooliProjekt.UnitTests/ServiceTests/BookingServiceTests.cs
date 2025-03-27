using KooliProjekt.Data;
using KooliProjekt.Services;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class BookingServiceTests : IAsyncLifetime
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

        // IAsyncLifetime liidesed meetodid: puhastame andmebaasi enne iga testi
        public async Task InitializeAsync()
        {
            // Eemalda kõik broneeringud enne iga testi
            _context.Bookings.RemoveRange(_context.Bookings);
            await _context.SaveChangesAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        // Save tests
        [Fact]
        public async Task Save_ShouldAddBooking_WhenBookingIsNew()
        {
            // Arrange: Lisa uus broneering, millel on ID 0 (tähendab, et broneering pole veel andmebaasis)
            var newBooking = new Booking
            {
                Id = 0,  // ID peab olema 0, et see oleks uus (nt kui on Identity või auto-korrektsioon)
                UserId = 1,
                CarId = 1,
                DistanceKm = 100,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1)
            };

            // Act: Salvesta uus broneering
            await _bookingService.Save(newBooking);

            // Assert: Broneering peab olema andmebaasis
            var savedBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.UserId == newBooking.UserId && b.CarId == newBooking.CarId);
            Assert.NotNull(savedBooking); // Veenduge, et broneering on salvestatud
            Assert.Equal(newBooking.UserId, savedBooking.UserId); // Kontrollige, et UserId on õige
            Assert.Equal(newBooking.CarId, savedBooking.CarId); // Kontrollige, et CarId on õige

            // Kui kasutate automaatset ID genereerimist (nt Identity), siis veenduge, et ID on määratud
            Assert.True(savedBooking.Id > 0); // ID peaks olema määratud ja suurem kui 0
        }

        // Otsingu test
        [Fact]
        public async Task Search_ShouldReturnCorrectBookings_WhenKeywordMatches()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, UserId = 1, CarId = 101, DistanceKm = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
                new Booking { Id = 2, UserId = 2, CarId = 102, DistanceKm = 200, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1) },
                new Booking { Id = 3, UserId = 3, CarId = 103, DistanceKm = 300, StartTime = DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddHours(1) }
            };

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            var search = new BookingSearch { Keyword = "101" }; // Otsing "101" (mis peaks vastama CarId-le)

            // Act
            var result = await _bookingService.Search(search); // See peaks olema sinu teenuse meetod, mis täidab koodilõigu

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Ainult üks broneering peaks olema, millel on CarId 101
            Assert.Equal(1, result.First().Id); // Peab olema broneering, mille ID on 1
        }

        // Delete tests
        [Fact]
        public async Task Delete_ShouldRemoveBooking_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { Id = 1, UserId = 1, CarId = 1, DistanceKm = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Act
            await _bookingService.Delete(booking.Id);

            // Assert
            var deletedBooking = await _context.Bookings.FindAsync(booking.Id);
            Assert.Null(deletedBooking); // Broneeringut ei tohiks enam olla
        }

        [Fact]
        public async Task Delete_ShouldNotThrow_WhenBookingDoesNotExist()
        {
            // Arrange
            var nonExistentId = 999; // ID, mis ei eksisteeri

            // Act
            var exception = await Record.ExceptionAsync(() => _bookingService.Delete(nonExistentId));

            // Assert
            Assert.Null(exception); // Erandit ei tohiks visata, kõik peaks toimuma ilma vigadeta
        }

        // Get tests
        [Fact]
        public async Task Get_ShouldReturnBooking_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { Id = 1, UserId = 1, CarId = 1, DistanceKm = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingService.Get(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(booking.Id, result.Id); // Kontrollige, et leiti õige broneering
        }

        [Fact]
        public async Task Get_ShouldReturnNull_WhenBookingDoesNotExist()
        {
            // Act
            var result = await _bookingService.Get(999); // ID, mis ei eksisteeri

            // Assert
            Assert.Null(result); // Broneeringut ei tohiks leida
        }

        // Includes tests
        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenBookingExists()
        {
            // Arrange
            var booking = new Booking { Id = 1, UserId = 1, CarId = 1, DistanceKm = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _bookingService.Includes(1);

            // Assert
            Assert.True(result); // Broneering peaks eksisteerima
        }

        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenBookingDoesNotExist()
        {
            // Act
            var result = await _bookingService.Includes(999); // ID, mis ei eksisteeri

            // Assert
            Assert.False(result); // Broneeringut ei tohiks leida
        }

        // List tests
        [Fact]
        public async Task List_ShouldReturnCorrectPagedResults_WhenPaginationIsApplied()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, UserId = 1, CarId = 1, DistanceKm = 100, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
                new Booking { Id = 2, UserId = 2, CarId = 2, DistanceKm = 200, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1) },
                new Booking { Id = 3, UserId = 3, CarId = 3, DistanceKm = 300, StartTime = DateTime.Now.AddDays(2), EndTime = DateTime.Now.AddDays(2).AddHours(1) },
                new Booking { Id = 4, UserId = 4, CarId = 4, DistanceKm = 400, StartTime = DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(3).AddHours(1) },
                new Booking { Id = 5, UserId = 5, CarId = 5, DistanceKm = 500, StartTime = DateTime.Now.AddDays(4), EndTime = DateTime.Now.AddDays(4).AddHours(1) }
            };

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();

            // Act: Get the first page with 2 items per page
            var result = await _bookingService.List(1, 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count); // Only 2 items should be returned (page 1)
            Assert.Equal(1, result.Results.First().Id); // First item should have Id = 1
            Assert.Equal(2, result.Results.Last().Id); // Last item on this page should have Id = 2
        }
    }
}
