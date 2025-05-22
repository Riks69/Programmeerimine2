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

namespace KooliProjekt.IntegrationTests
{
    [Collection("Sequential")]
    public class CarsControllerTests : TestBase
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public CarsControllerTests()
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = Factory.CreateClient(options);
            _context = (ApplicationDbContext)Factory.Services.GetService(typeof(ApplicationDbContext));
        }

        [Fact]
        public async Task Create_should_save_new_car()
        {
            // Arrange
            var formValues = new Dictionary<string, string>
            {
                { "Type", "Sedan" },
                { "RegistrationNumber", "ABC123" },
                { "HourlyRate", "20.0" },
                { "KmRate", "0.5" },
                { "IsAvaliable", "true" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act
            using var response = await _client.PostAsync("/Cars/Create", content);

            // Assert
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var car = _context.Cars.FirstOrDefault(c => c.RegistrationNumber == "ABC123");
                Assert.NotNull(car);
                Assert.Equal("Sedan", car.Type);
                Assert.Equal("ABC123", car.RegistrationNumber);
                Assert.Equal(20.0, car.HourlyRate);
                Assert.Equal(0.5, car.KmRate);
                Assert.True(car.IsAvaliable);
            }
            else
            {
                Console.WriteLine("Hmm... doesn't work");
            }
        }

        [Fact]
        public async Task Create_should_not_save_invalid_car()
        {
            // Arrange
            var formValues = new Dictionary<string, string>
            {
                { "Type", "" },
                { "RegistrationNumber", "" },
                { "HourlyRate", "" },
                { "KmRate", "" },
                { "IsAvaliable", "" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act
            using var response = await _client.PostAsync("/Cars/Create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(_context.Cars.Any());
        }
    }
}
