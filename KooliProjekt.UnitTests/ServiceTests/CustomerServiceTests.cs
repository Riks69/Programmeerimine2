using KooliProjekt.Data;
using KooliProjekt.Services;
using KooliProjekt.Models;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class CustomerServiceTests : ServiceTestBase
    {
        private readonly ICustomerService _customerService;

        public CustomerServiceTests()
        {
            _customerService = new CustomerService(DbContext);  // Kutsume üles CustomerService testimiseks
        }

        // Test, et auto lisamine töötab ja toob õiged andmed
        [Fact]
        public async Task Save_ShouldAddCustomer()
        {
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                Id = 0  // ID 0 tähendab, et see on uus klient
            };

            await _customerService.Save(customer);

            var createdCustomer = await DbContext.Customers.FindAsync(customer.Id);
            Assert.NotNull(createdCustomer);
            Assert.Equal("John Doe", createdCustomer.Name);
            Assert.Equal("john.doe@example.com", createdCustomer.Email);
            Assert.Equal("password123", createdCustomer.Password);
            Assert.True(createdCustomer.Id > 0);  // Kontrollime, et ID on määratud pärast salvestamist
        }

        // Test, et klienti saab uuendada
        [Fact]
        public async Task Save_ShouldUpdateCustomer_WhenCustomerExists()
        {
            // Loome esmalt kliendi
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password123"
            };
            await _customerService.Save(customer);

            // Uuendame kliendi andmed
            customer.Name = "Jane Doe";
            customer.Email = "jane.doe@example.com";
            customer.Password = "newpassword456";

            await _customerService.Save(customer);

            var updatedCustomer = await _customerService.Get(customer.Id);
            Assert.NotNull(updatedCustomer);
            Assert.Equal("Jane Doe", updatedCustomer.Name);
            Assert.Equal("jane.doe@example.com", updatedCustomer.Email);
            Assert.Equal("newpassword456", updatedCustomer.Password);
        }

        // Test, et klient kustutatakse õigesti
        [Fact]
        public async Task Delete_ShouldRemoveCustomer()
        {
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password123"
            };
            await _customerService.Save(customer);

            await _customerService.Delete(customer.Id);

            var deletedCustomer = await DbContext.Customers.FindAsync(customer.Id);
            Assert.Null(deletedCustomer);  // Kontrollime, et klient on andmebaasist kustutatud
        }

        // Test, et Includes töötab õigesti
        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenCustomerExists()
        {
            var customer = new Customer
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = "password123"
            };
            await _customerService.Save(customer);

            var exists = await _customerService.Includes(customer.Id);

            Assert.True(exists);  // Kontrollime, et klient eksisteerib
        }

        // Test, et Includes tagastab false, kui klient ei eksisteeri
        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            var exists = await _customerService.Includes(999);  // Kliendi ID, mida pole olemas

            Assert.False(exists);  // Kontrollime, et klient ei eksisteeri
        }

        // Test, et List töötab õigesti
        [Fact]
        public async Task List_ShouldReturnPagedCustomers_WhenKeywordMatches()
        {
            var customer1 = new Customer { Name = "John Doe", Email = "john.doe@example.com", Password = "password123" };
            var customer2 = new Customer { Name = "Jane Doe", Email = "jane.doe@example.com", Password = "newpassword456" };
            await _customerService.Save(customer1);
            await _customerService.Save(customer2);

            var search = new CustomerSearch { Keyword = "John" };

            var result = await _customerService.List(1, 10, search);

            Assert.NotNull(result);
            Assert.Equal(1, result.Results.Count);
            Assert.Contains(result.Results, c => c.Name == "John Doe");
        }

        // Test, et List tagastab kõik kliendid, kui otsing on tühi
        [Fact]
        public async Task List_ShouldReturnAllCustomers_WhenSearchIsNull()
        {
            var customer1 = new Customer { Name = "John Doe", Email = "john.doe@example.com", Password = "password123" };
            var customer2 = new Customer { Name = "Jane Doe", Email = "jane.doe@example.com", Password = "newpassword456" };
            await _customerService.Save(customer1);
            await _customerService.Save(customer2);

            var result = await _customerService.List(1, 10, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);  // Kõik kliendid peaksid olema tagastatud
        }
    }
}
