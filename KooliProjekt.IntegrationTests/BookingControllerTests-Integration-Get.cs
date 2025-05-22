using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using KooliProjekt.Data;
using KooliProjekt.IntegrationTests.Helpers;
using Xunit;
using static NuGet.Packaging.PackagingConstants;

namespace KooliProjekt.IntegrationTests
{
    [Collection("Sequential")]
    public class BookingControllerTests : TestBase
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public BookingControllerTests()
        {
            _client = Factory.CreateClient();
            _context = (ApplicationDbContext)Factory.Services.GetService(typeof(ApplicationDbContext));
        }

        [Fact]
        public async Task Index_should_return_correct_response()
        {
            // Arrange

            // Act
            using var response = await _client.GetAsync("/Bookings");

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Details_should_return_notfound_when_list_was_not_found()
        {
            // Arrange

            // Act
            using var response = await _client.GetAsync("/Bookings/Details/100");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Details_should_return_notfound_when_id_is_missing()
        {
            // Arrange

            // Act
            using var response = await _client.GetAsync("/Bookings/Details/");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Details_should_return_ok_when_list_was_found()
        {
            // Arrange
            var list = new Booking { UserId = 1, StartTime = new DateTime(2024, 11, 25, 9, 0, 0), EndTime = new DateTime(2024, 11, 28, 9, 0, 0), DistanceKm = 200, IsCompleted = true };
            _context.Bookings.Add(list);
            _context.SaveChanges();

            // Act
            using var response = await _client.GetAsync("/Bookings/Details/" + list.Id);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}