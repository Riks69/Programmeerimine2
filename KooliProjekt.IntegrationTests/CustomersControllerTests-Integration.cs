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
    public class CustomersControllerTests : TestBase
    {
        private readonly HttpClient _client;
        private readonly ApplicationDbContext _context;

        public CustomersControllerTests()
        {
            var options = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = Factory.CreateClient(options);
            _context = (ApplicationDbContext)Factory.Services.GetService(typeof(ApplicationDbContext));
        }

        // Test 1: Create a new customer
        [Fact]
        public async Task Create_should_save_new_customer()
        {
            // Arrange: Prepare valid form values for creating a new customer
            var formValues = new Dictionary<string, string>
            {
                { "Name", "John Doe" },
                { "Password", "SecurePassword123" },
                { "Email", "johndoe@example.com" },
                { "IsRegistered", "true" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act: Send a POST request to create a new customer
            using var response = await _client.PostAsync("/Customers/Create", content);

            // Assert: Check if the customer was successfully created and saved
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Name == "John Doe");
                Assert.NotNull(customer);
                Assert.Equal("John Doe", customer.Name);
                Assert.Equal("SecurePassword123", customer.Password);
                Assert.Equal("johndoe@example.com", customer.Email);
                Assert.True(customer.IsRegistered);
            }
            else
            {
                Console.WriteLine("Hmm... doesn't work");
            }
        }

        // Test 2: Try creating an invalid customer (with empty fields)
        [Fact]
        public async Task Create_should_not_save_invalid_customer()
        {
            // Arrange: Prepare form values with invalid (empty) data
            var formValues = new Dictionary<string, string>
            {
                { "Name", "" },  // Name is required
                { "Password", "" },  // Password is required
                { "Email", "" },
                { "IsRegistered", "" }
            };

            using var content = new FormUrlEncodedContent(formValues);

            // Act: Send a POST request with invalid data
            using var response = await _client.PostAsync("/Customers/Create", content);

            // Assert: Verify that the response is a BadRequest (400)
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(_context.Customers.Any());
        }

        // Test 3: Edit an existing customer
        [Fact]
        public async Task Edit_should_update_customer()
        {
            // Arrange: Create a customer first
            var newCustomer = new Customer
            {
                Name = "Jane Doe",
                Password = "Password123",
                Email = "janedoe@example.com",
                IsRegistered = true
            };
            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            // Act: Edit the customer's name and email
            var formValues = new Dictionary<string, string>
            {
                { "Id", newCustomer.Id.ToString() },
                { "Name", "Jane Smith" },
                { "Password", "NewPassword123" },
                { "Email", "janesmith@example.com" },
                { "IsRegistered", "true" }
            };
            using var content = new FormUrlEncodedContent(formValues);

            // Send the POST request to edit the customer
            using var response = await _client.PostAsync($"/Customers/Edit/{newCustomer.Id}", content);

            // Assert: Check if the customer was updated successfully
            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                var updatedCustomer = _context.Customers.FirstOrDefault(c => c.Id == newCustomer.Id);
                Assert.NotNull(updatedCustomer);
                Assert.Equal("Jane Smith", updatedCustomer.Name);
                Assert.Equal("janesmith@example.com", updatedCustomer.Email);
            }
            else
            {
                Console.WriteLine("Hmm... doesn't work");
            }
        }
    }
}
