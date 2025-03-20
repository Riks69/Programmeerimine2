using KooliProjekt.Controllers;
using KooliProjekt.Data;
using KooliProjekt.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ControllerTests
{
    public class CustomersControllerTests
    {
        private readonly CustomersController _controller;
        private readonly Mock<ICustomerService> _customerServiceMock;

        public CustomersControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _controller = new CustomersController(_customerServiceMock.Object);
        }

        // Details Tests

        [Fact]
        public async Task Details_should_return_notfound_when_id_is_null()
        {
            int? id = null;

            var result = await _controller.Details(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_should_return_not_found_when_customer_does_not_exist()
        {
            var customerId = 999;
            _customerServiceMock.Setup(x => x.Get(customerId)).ReturnsAsync((Customer)null);

            var result = await _controller.Details(customerId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_should_return_view_when_customer_exists()
        {
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "John Doe", Password = "password", IsRegistered = true };
            _customerServiceMock.Setup(x => x.Get(customerId)).ReturnsAsync(customer);

            var result = await _controller.Details(customerId) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<Customer>(result.Model);
            Assert.Equal(customerId, model.Id);
        }

        // Create Tests

        [Fact]
        public void Create_should_return_view()
        {
            var result = _controller.Create() as ViewResult;

            Assert.NotNull(result);
            Assert.Null(result.ViewName); // Veenduge, et ei ole määratud eraldi vaate nime
        }

        [Fact]
        public async Task Create_should_return_view_when_model_is_invalid()
        {
            var customer = new Customer { Id = 1, Name = "John", Password = "password", IsRegistered = true };
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = await _controller.Create(customer) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(customer, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_should_redirect_to_index_when_model_is_valid()
        {
            var customer = new Customer { Id = 1, Name = "John", Password = "password", IsRegistered = true };
            _customerServiceMock.Setup(x => x.Save(It.IsAny<Customer>())).Returns(Task.CompletedTask);

            var result = await _controller.Create(customer) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _customerServiceMock.Verify(x => x.Save(It.IsAny<Customer>()), Times.Once);
        }

        // Test email validation (empty and invalid)

        [Fact]
        public async Task Create_should_return_view_when_email_is_missing()
        {
            var customer = new Customer
            {
                Id = 1,
                Name = "John",
                Password = "password",
                IsRegistered = true,
                Email = string.Empty // Email on tühi, mis peaks põhjustama viga
            };

            _controller.ModelState.AddModelError("Email", "The Email field is required.");

            var result = await _controller.Create(customer) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(customer, result.Model);
            Assert.False(_controller.ModelState.IsValid); // Veenduge, et ModelState ei ole kehtiv, kuna email on puudu
        }

        [Fact]
        public async Task Create_should_return_view_when_email_is_invalid()
        {
            var customer = new Customer
            {
                Id = 1,
                Name = "John",
                Password = "password",
                IsRegistered = true,
                Email = "invalid-email" // Ebaõige emaili formaat
            };

            _controller.ModelState.AddModelError("Email", "The Email field is not a valid e-mail address.");

            var result = await _controller.Create(customer) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(customer, result.Model);
            Assert.False(_controller.ModelState.IsValid); // Veenduge, et ModelState ei ole kehtiv, kuna email on vale formaadiga
        }

        // Edit Tests (GET)

        [Fact]
        public async Task Edit_should_return_notfound_when_id_is_null()
        {
            int? id = null;

            var result = await _controller.Edit(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_should_return_notfound_when_customer_does_not_exist()
        {
            var customerId = 999;
            _customerServiceMock.Setup(x => x.Get(customerId)).ReturnsAsync((Customer)null);

            var result = await _controller.Edit(customerId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_should_return_view_when_customer_exists()
        {
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "John Doe", Password = "password", IsRegistered = true };
            _customerServiceMock.Setup(x => x.Get(customerId)).ReturnsAsync(customer);

            var result = await _controller.Edit(customerId) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<Customer>(result.Model);
            Assert.Equal(customerId, model.Id);
        }

        // Edit Tests (POST)

        [Fact]
        public async Task Edit_should_return_notfound_when_id_mismatch()
        {
            var customerId = 1;
            var customer = new Customer { Id = 2, Name = "Jane Doe", Password = "newpassword", IsRegistered = true };

            var result = await _controller.Edit(customerId, customer);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_should_redirect_to_index_when_model_is_valid()
        {
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "John Doe", Password = "password", IsRegistered = true };
            _customerServiceMock.Setup(x => x.Save(It.IsAny<Customer>())).Returns(Task.CompletedTask);

            var result = await _controller.Edit(customerId, customer) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _customerServiceMock.Verify(x => x.Save(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task Edit_should_return_view_when_model_is_invalid()
        {
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "John Doe", Password = "password", IsRegistered = true };
            _controller.ModelState.AddModelError("Name", "Name is required");

            var result = await _controller.Edit(customerId, customer) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(customer, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        // Delete Tests

        [Fact]
        public async Task DeleteConfirmed_should_redirect_to_index()
        {
            var customerId = 1;
            _customerServiceMock.Setup(x => x.Delete(customerId)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(customerId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _customerServiceMock.Verify(x => x.Delete(customerId), Times.Once);
        }

        [Fact]
        public async Task Delete_should_return_notfound_when_id_is_null()
        {
            int? id = null;

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_notfound_when_customer_does_not_exist()
        {
            var customerId = 999;
            _customerServiceMock.Setup(x => x.Get(customerId)).ReturnsAsync((Customer)null);

            var result = await _controller.Delete(customerId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_view_when_customer_exists()
        {
            var customerId = 1;
            var customer = new Customer { Id = customerId, Name = "John Doe", Password = "password", IsRegistered = true };
            _customerServiceMock.Setup(x => x.Get(customerId)).ReturnsAsync(customer);

            var result = await _controller.Delete(customerId) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<Customer>(result.Model);
            Assert.Equal(customerId, model.Id);
        }
    }
}
