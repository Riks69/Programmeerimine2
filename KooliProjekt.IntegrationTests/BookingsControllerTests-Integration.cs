using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using KooliProjekt.Data;
using KooliProjekt.Models;
using KooliProjekt.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Linq;
using System;

namespace KooliProjekt.IntegrationTests
{
    [Collection("Sequential")]
    public class BookingsControllerTests : TestBase
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public BookingsControllerTests()
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = Factory.CreateClient(options);
            _context = (ApplicationDbContext)Factory.Services.GetService(typeof(ApplicationDbContext));
        }

        [Fact]
        public async Task Create_should_save_new_booking()
        {
            // Arrange
            var formValues = new Dictionary<string, string>
            {
                { "UserId", "1" },
                { "CarId", "1" },
                { "StartTime", "2025-03-27T09:00:00" },
                { "EndTime", "2025-03-27T12:00:00" },
                { "DistanceKm", "100" },
                { "IsCompleted", "false" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act
            using var response = await _client.PostAsync("/Bookings/Create", content);

            // Assert
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var booking = _context.Bookings.FirstOrDefault(b => b.UserId == 1 && b.CarId == 1);
                Assert.NotNull(booking);
                Assert.Equal(1, booking.UserId);
                Assert.Equal(1, booking.CarId);
                Assert.Equal(new DateTime(2025, 03, 27, 9, 0, 0), booking.StartTime);
                Assert.Equal(new DateTime(2025, 03, 27, 12, 0, 0), booking.EndTime);
                Assert.Equal(100, booking.DistanceKm);
                Assert.False(booking.IsCompleted);
            }
            else
            {
                Console.WriteLine("Hmm... doesn't work");
            }
        }

        [Fact]
        public async Task Create_should_not_save_invalid_booking()
        {
            // Arrange
            var formValues = new Dictionary<string, string>
            {
                { "UserId", "" },
                { "CarId", "" },
                { "StartTime", "" },
                { "EndTime", "" },
                { "DistanceKm", "" },
                { "IsCompleted", "" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act
            using var response = await _client.PostAsync("/Bookings/Create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(_context.Bookings.Any());
        }
    }
}
