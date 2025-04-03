using KooliProjekt.Data;
using KooliProjekt.Services;
using KooliProjekt.Models;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class BookingServiceTests : ServiceTestBase
    {
        private readonly IBookingService _bookingService;

        public BookingServiceTests()
        {
            _bookingService = new BookingService(DbContext);
        }

        // Test, et broneeringu lisamine töötab ja toob õiged andmed
        [Fact]
        public async Task Create_ShouldAddBooking()
        {
            // Arrange
            var booking = new Booking
            {
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100,
                Id = 0
            };

            // Act
            await _bookingService.Save(booking);

            // Assert
            var createdBooking = await DbContext.Bookings.FindAsync(booking.Id);
            Assert.NotNull(createdBooking);
            Assert.Equal(1, createdBooking.UserId);
            Assert.Equal(2, createdBooking.CarId);
            Assert.Equal(100, createdBooking.DistanceKm);
            Assert.True(createdBooking.Id > 0);
        }

        // Test lehtede vaatamiseks ja kontrollimiseks, kas broneeringud on õigesti tagastatud
        [Fact]
        public async Task List_ShouldReturnPagedBookings()
        {
            var booking1 = new Booking { UserId = 1, CarId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), DistanceKm = 100 };
            var booking2 = new Booking { UserId = 2, CarId = 3, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1), DistanceKm = 200 };
            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            var search = new BookingSearch { Keyword = "1" };

            var result = await _bookingService.List(1, 10, search);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);
        }

        [Fact]
        public async Task List_ShouldReturnAllBookings_WhenSearchIsNull()
        {
            // Arrange
            var booking1 = new Booking { UserId = 1, CarId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), DistanceKm = 100 };
            var booking2 = new Booking { UserId = 2, CarId = 3, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1), DistanceKm = 200 };
            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            // Act
            var result = await _bookingService.List(1, 10, null); // Otsing on null

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count); // Kõik broneeringud peaksid olema tagastatud
        }


        // Test, et õige broneering tagastatakse ID järgi
        [Fact]
        public async Task Get_ShouldReturnBookingById()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100
            };
            await _bookingService.Save(booking);

            var fetchedBooking = await _bookingService.Get(booking.Id);

            Assert.NotNull(fetchedBooking);
            Assert.Equal(1, fetchedBooking.UserId);
            Assert.Equal(2, fetchedBooking.CarId);
            Assert.Equal(100, fetchedBooking.DistanceKm);
        }

        // Test, et broneeringut saab uuendada
        [Fact]
        public async Task Save_ShouldUpdateBooking()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100
            };
            await _bookingService.Save(booking);

            booking.UserId = 3;
            booking.CarId = 4;
            booking.DistanceKm = 150;
            await _bookingService.Save(booking);

            var updatedBooking = await _bookingService.Get(booking.Id);
            Assert.NotNull(updatedBooking);
            Assert.Equal(3, updatedBooking.UserId);
            Assert.Equal(4, updatedBooking.CarId);
            Assert.Equal(150, updatedBooking.DistanceKm);
        }

        [Fact]
        public async Task Save_ShouldNotUpdateBooking_WhenBookingDoesNotExist()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 999, // See ID ei ole andmebaasis olemas
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100
            };

            // Veendume, et broneeringut pole andmebaasis
            var initialBooking = await _bookingService.Get(booking.Id);
            Assert.Null(initialBooking);

            // Act
            await _bookingService.Save(booking); // Save peaks kontrollima, et broneeringut ei leita ja mitte midagi ei tee

            // Assert
            // Kontrollime, et broneering ei ole andmebaasis, kuna ID-ga broneeringut ei leitud
            var updatedBooking = await _bookingService.Get(booking.Id);
            Assert.Null(updatedBooking); // Kui midagi ei tehta, siis broneeringut ei lisata ega uuendata
        }


        // Test, et broneering eemaldatakse õigesti
        [Fact]
        public async Task Delete_ShouldRemoveBooking()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100
            };
            await _bookingService.Save(booking);

            await _bookingService.Delete(booking.Id);

            var deletedBooking = await DbContext.Bookings.FindAsync(booking.Id);
            Assert.Null(deletedBooking);
        }

        // Test, et Includes tagastab õigesti true, kui broneering olemas
        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenBookingExists()
        {
            var booking = new Booking
            {
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100
            };
            await _bookingService.Save(booking);

            var exists = await _bookingService.Includes(booking.Id);

            Assert.True(exists);
        }

        // Test, et Includes tagastab false, kui broneeringut ei eksisteeri
        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenBookingDoesNotExist()
        {
            var exists = await _bookingService.Includes(999);

            Assert.False(exists);
        }

        [Fact]
        public async Task Search_ShouldReturnBookings_WhenKeywordAndDoneStatusMatch()
        {
            // Arrange
            var booking1 = new Booking
            {
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100,
                IsCompleted = true // Lõpetatud broneering
            };

            var booking2 = new Booking
            {
                UserId = 2,
                CarId = 3,
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                DistanceKm = 200,
                IsCompleted = false // Mitte-lõpetatud broneering
            };

            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            // Otsing Keyword järgi ja Done= true järgi (lõpetatud broneering)
            var search = new BookingSearch { Keyword = "1", Done = true };

            var result = await _bookingService.Search(search);

            Assert.NotNull(result);
            Assert.Single(result); // Ainult üks broneering, kuna otsing peab leidma ainult lõpetatud broneeringu, mis sisaldab Keyword "1"
            Assert.Contains(result, b => b.IsCompleted == true && b.UserId == 1);
        }


        [Fact]
        public async Task Search_ShouldReturnBookings_WhenNotDoneStatusMatches()
        {
            // Arrange
            var booking1 = new Booking
            {
                UserId = 1,
                CarId = 2,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(1),
                DistanceKm = 100,
                IsCompleted = false // Mitte-lõpetatud broneering
            };

            var booking2 = new Booking
            {
                UserId = 2,
                CarId = 3,
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                DistanceKm = 200,
                IsCompleted = true // Lõpetatud broneering
            };

            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            // Otsing mittetäidetud broneeringute järgi
            var search = new BookingSearch { Done = false };

            var result = await _bookingService.Search(search);

            Assert.NotNull(result);
            Assert.Single(result);  // Ainult üks broneering, sest otsime ainult mittetäidetud broneeringuid
            Assert.Contains(result, b => b.IsCompleted == false);
        }

        [Fact]
        public async Task Search_ShouldReturnAllBookings_WhenSearchIsNull()
        {
            // Arrange
            var booking1 = new Booking { UserId = 1, CarId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), DistanceKm = 100, IsCompleted = true };
            var booking2 = new Booking { UserId = 2, CarId = 3, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1), DistanceKm = 200, IsCompleted = false };
            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            // Act
            var result = await _bookingService.Search(null); // search on null

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Kõik broneeringud peaksid olema tagastatud
        }

        [Fact]
        public async Task Search_ShouldReturnAllBookings_WhenKeywordIsEmpty()
        {
            // Arrange
            var booking1 = new Booking { UserId = 1, CarId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), DistanceKm = 100, IsCompleted = true };
            var booking2 = new Booking { UserId = 2, CarId = 3, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1), DistanceKm = 200, IsCompleted = false };
            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            var search = new BookingSearch { Keyword = "" }; // Keyword on tühi

            // Act
            var result = await _bookingService.Search(search);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Kõik broneeringud peaksid olema tagastatud
        }

        [Fact]
        public async Task Search_ShouldReturnAllBookings_WhenDoneIsNull()
        {
            // Arrange
            var booking1 = new Booking { UserId = 1, CarId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), DistanceKm = 100, IsCompleted = true };
            var booking2 = new Booking { UserId = 2, CarId = 3, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1), DistanceKm = 200, IsCompleted = false };
            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            var search = new BookingSearch { Done = null }; // Done on null

            // Act
            var result = await _bookingService.Search(search);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Kõik broneeringud peaksid olema tagastatud
        }

        [Fact]
        public async Task Search_ShouldReturnCompletedBookings_WhenDoneIsTrue()
        {
            // Arrange
            var booking1 = new Booking { UserId = 1, CarId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), DistanceKm = 100, IsCompleted = true };
            var booking2 = new Booking { UserId = 2, CarId = 3, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1), DistanceKm = 200, IsCompleted = false };
            await _bookingService.Save(booking1);
            await _bookingService.Save(booking2);

            var search = new BookingSearch { Done = true }; // Done = true

            // Act
            var result = await _bookingService.Search(search);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Ainult üks broneering, mis on lõpetatud
            Assert.Contains(result, b => b.IsCompleted == true);
        }

        [Fact]
public async Task Search_ShouldReturnUncompletedBookings_WhenDoneIsFalse()
{
    // Arrange
    var booking1 = new Booking { UserId = 1, CarId = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), DistanceKm = 100, IsCompleted = true };
    var booking2 = new Booking { UserId = 2, CarId = 3, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(1), DistanceKm = 200, IsCompleted = false };
    await _bookingService.Save(booking1);
    await _bookingService.Save(booking2);

    var search = new BookingSearch { Done = false }; // Done = false

    // Act
    var result = await _bookingService.Search(search);

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);  // Ainult üks broneering, mis on lõpetamata
    Assert.Contains(result, b => b.IsCompleted == false);
}

    }
}
