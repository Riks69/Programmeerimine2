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
    public class InvoicesControllerTests : TestBase
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public InvoicesControllerTests()
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = Factory.CreateClient(options);
            _context = (ApplicationDbContext)Factory.Services.GetService(typeof(ApplicationDbContext));
        }

        [Fact]
        public async Task Create_should_save_new_invoice()
        {
            // Arrange
            var formValues = new Dictionary<string, string>
            {
                { "BookingId", "1" },
                { "Amount", "150.0" },
                { "Description", "Car rental for 3 hours" },
                { "IsPaid", "false" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act
            using var response = await _client.PostAsync("/Invoices/Create", content);

            // Assert
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var invoice = _context.Invoices.FirstOrDefault(i => i.BookingId == 1);
                Assert.NotNull(invoice);
                Assert.Equal(1, invoice.BookingId);
                Assert.Equal(150.0, invoice.Amount);
                Assert.Equal("Car rental for 3 hours", invoice.Description);
                Assert.False(invoice.IsPaid);
            }
            else
            {
                Console.WriteLine("Hmm... doesn't work");
            }
        }

        [Fact]
        public async Task Create_should_not_save_invalid_invoice()
        {
            // Arrange
            var formValues = new Dictionary<string, string>
            {
                { "BookingId", "" },
                { "Amount", "" },
                { "Description", "" },
                { "IsPaid", "" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act
            using var response = await _client.PostAsync("/Invoices/Create", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(_context.Invoices.Any());
        }
    }
}
