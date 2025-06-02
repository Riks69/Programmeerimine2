using KooliProjekt.Data;
using KooliProjekt.Models;
using KooliProjekt.Search;
using KooliProjekt.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class CustomerServiceTests : ServiceTestBase
    {
        private readonly ICustomerService _customerService;

        public CustomerServiceTests()
        {
            _customerService = new CustomerService(DbContext);
        }

        [Fact]
        public async Task Save_ShouldAddCustomer_WhenIdIsZero()
        {
            var customer = new Customer
            {
                Name = "John",
                Email = "john@example.com",
                Password = "password123",
                IsRegistered = true
            };

            await _customerService.Save(customer);

            var saved = await DbContext.Customers.FindAsync(customer.Id);
            Assert.NotNull(saved);
            Assert.Equal("John", saved.Name);
        }

        [Fact]
        public async Task Save_ShouldUpdateCustomer_WhenExists()
        {
            var customer = new Customer
            {
                Name = "Anna",
                Email = "anna@example.com",
                Password = "123456",
                IsRegistered = false
            };
            await _customerService.Save(customer);

            customer.Name = "Anna Maria";
            customer.IsRegistered = true;
            await _customerService.Save(customer);

            var updated = await _customerService.Get(customer.Id);
            Assert.Equal("Anna Maria", updated.Name);
            Assert.True(updated.IsRegistered);
        }

        [Fact]
        public async Task Save_ShouldDoNothing_WhenCustomerDoesNotExist()
        {
            var customer = new Customer
            {
                Id = 999,
                Name = "Ghost",
                Email = "ghost@none.com",
                Password = "nopass",
                IsRegistered = false
            };

            await _customerService.Save(customer);

            var check = await _customerService.Get(customer.Id);
            Assert.Null(check); // ei tohiks lisanduda
        }

        [Fact]
        public async Task Delete_ShouldRemoveCustomer()
        {
            var customer = new Customer
            {
                Name = "ToDelete",
                Email = "delete@me.com",
                Password = "del123",
                IsRegistered = true
            };
            await _customerService.Save(customer);

            await _customerService.Delete(customer.Id);

            var deleted = await _customerService.Get(customer.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Get_ShouldReturnCustomer_WhenExists()
        {
            var customer = new Customer
            {
                Name = "Getter",
                Email = "getter@example.com",
                Password = "pass",
                IsRegistered = true
            };
            await _customerService.Save(customer);

            var result = await _customerService.Get(customer.Id);
            Assert.NotNull(result);
            Assert.Equal("Getter", result.Name);
        }

        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenCustomerExists()
        {
            var customer = new Customer { Name = "Test", Email = "a@a.com", Password = "123", IsRegistered = true };
            await _customerService.Save(customer);

            var exists = await _customerService.Includes(customer.Id);
            Assert.True(exists);
        }

        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenCustomerNotExists()
        {
            var exists = await _customerService.Includes(999);
            Assert.False(exists);
        }

        [Fact]
        public async Task List_ShouldReturnPagedCustomers()
        {
            await _customerService.Save(new Customer { Name = "Alice", Email = "a@a.com", Password = "pass", IsRegistered = true });
            await _customerService.Save(new Customer { Name = "Bob", Email = "b@b.com", Password = "pass", IsRegistered = false });

            var result = await _customerService.List(1, 10, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);
        }

        [Fact]
        public async Task List_ShouldFilterByKeyword()
        {
            await _customerService.Save(new Customer { Name = "Keyword", Email = "key@word.com", Password = "key", IsRegistered = true });
            await _customerService.Save(new Customer { Name = "NoMatch", Email = "no@match.com", Password = "pass", IsRegistered = false });

            var result = await _customerService.List(1, 10, new CustomerSearch { Keyword = "key" });

            Assert.Single(result.Results);
            Assert.Contains(result.Results, c => c.Name.Contains("Keyword"));
        }

        [Fact]
        public async Task Search_ShouldReturnMatchingCustomers_ByKeyword()
        {
            await _customerService.Save(new Customer { Name = "Marek", Email = "marek@example.com", Password = "test", IsRegistered = true });
            await _customerService.Save(new Customer { Name = "Teet", Email = "teet@example.com", Password = "secret", IsRegistered = false });

            var search = new CustomerSearch { Keyword = "marek" };

            var result = await _customerService.Search(search);

            Assert.Single(result);
            Assert.Contains(result, c => c.Name == "Marek");
        }

        [Fact]
        public async Task Search_ShouldReturnMatchingCustomers_ByDoneTrue()
        {
            await _customerService.Save(new Customer { Name = "RegUser", Email = "reg@user.com", Password = "pass", IsRegistered = true });
            await _customerService.Save(new Customer { Name = "UnregUser", Email = "unreg@user.com", Password = "pass", IsRegistered = false });

            var search = new CustomerSearch { Done = true };

            var result = await _customerService.Search(search);

            Assert.Single(result);
            Assert.All(result, c => Assert.True(c.IsRegistered));
        }

        [Fact]
        public async Task Search_ShouldReturnMatchingCustomers_ByDoneFalse()
        {
            await _customerService.Save(new Customer { Name = "RegUser", Email = "reg@user.com", Password = "pass", IsRegistered = true });
            await _customerService.Save(new Customer { Name = "UnregUser", Email = "unreg@user.com", Password = "pass", IsRegistered = false });

            var search = new CustomerSearch { Done = false };

            var result = await _customerService.Search(search);

            Assert.Single(result);
            Assert.All(result, c => Assert.False(c.IsRegistered));
        }

        [Fact]
        public async Task Search_ShouldReturnAll_WhenSearchIsNull()
        {
            await _customerService.Save(new Customer { Name = "A", Email = "a@a.com", Password = "pass", IsRegistered = true });
            await _customerService.Save(new Customer { Name = "B", Email = "b@b.com", Password = "pass", IsRegistered = false });

            var result = await _customerService.Search(null);

            Assert.Equal(2, result.Count);
        }
    }
}
