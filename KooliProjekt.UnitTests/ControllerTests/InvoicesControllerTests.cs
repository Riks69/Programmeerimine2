using KooliProjekt.Controllers;
using KooliProjekt.Data;
using KooliProjekt.Services;
using Microsoft.AspNetCore.Mvc;
using KooliProjekt.Models;
using KooliProjekt.Search;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ControllerTests
{
    public class InvoicesControllerTests
    {
        private readonly InvoicesController _controller;
        private readonly Mock<IInvoiceService> _invoiceServiceMock;

        public InvoicesControllerTests()
        {
            _invoiceServiceMock = new Mock<IInvoiceService>();
            _controller = new InvoicesController(_invoiceServiceMock.Object);
        }

        // Index Tests
        [Fact]
        public async Task Index_should_return_view_with_invoices()
        {
            var invoices = new PagedResult<Invoice>
            {
                Results = new List<Invoice>
                {
                    new Invoice { Id = 1, BookingId = 101, Amount = 100.50, Description = "Test Invoice 1", IsPaid = false },
                    new Invoice { Id = 2, BookingId = 102, Amount = 200.75, Description = "Test Invoice 2", IsPaid = true },
                }
            };
            _invoiceServiceMock.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(invoices);

            var result = await _controller.Index() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<InvoiceIndexModel>(result.Model);
            Assert.Equal(invoices.Results.Count, model.Data.Results.Count);
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
        public async Task Details_should_return_not_found_when_invoice_does_not_exist()
        {
            var invoiceId = 999;
            _invoiceServiceMock.Setup(x => x.Get(invoiceId)).ReturnsAsync((Invoice)null);
            var result = await _controller.Details(invoiceId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_should_return_view_when_invoice_exists()
        {
            var invoiceId = 1;
            var invoice = new Invoice { Id = invoiceId, BookingId = 101, Amount = 100.50, Description = "Test Invoice", IsPaid = false };
            _invoiceServiceMock.Setup(x => x.Get(invoiceId)).ReturnsAsync(invoice);
            var result = await _controller.Details(invoiceId) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Invoice>(result.Model);
            Assert.Equal(invoiceId, model.Id);
        }

        // Create Tests
        [Fact]
        public void Create_should_return_view()
        {
            var result = _controller.Create() as ViewResult;
            Assert.NotNull(result);
            Assert.Null(result.ViewName);
        }

        [Fact]
        public async Task Create_should_return_view_when_model_is_invalid()
        {
            var invoice = new Invoice { Id = 1, BookingId = 101, Amount = 100.50, Description = "Test Invoice", IsPaid = false };
            _controller.ModelState.AddModelError("Description", "Description is required");
            var result = await _controller.Create(invoice) as ViewResult;
            Assert.NotNull(result);
            Assert.Equal(invoice, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_should_redirect_to_index_when_model_is_valid()
        {
            var invoice = new Invoice { Id = 1, BookingId = 101, Amount = 100.50, Description = "Test Invoice", IsPaid = false };
            _invoiceServiceMock.Setup(x => x.Save(It.IsAny<Invoice>())).Returns(Task.CompletedTask);
            var result = await _controller.Create(invoice) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _invoiceServiceMock.Verify(x => x.Save(It.IsAny<Invoice>()), Times.Once);
        }

        // Delete Tests
        [Fact]
        public async Task DeleteConfirmed_should_redirect_to_index()
        {
            var invoiceId = 1;
            _invoiceServiceMock.Setup(x => x.Delete(invoiceId)).Returns(Task.CompletedTask);
            var result = await _controller.DeleteConfirmed(invoiceId) as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _invoiceServiceMock.Verify(x => x.Delete(invoiceId), Times.Once);
        }

        [Fact]
        public async Task Delete_should_return_notfound_when_id_is_null()
        {
            int? id = null;
            var result = await _controller.Delete(id);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_notfound_when_invoice_does_not_exist()
        {
            var invoiceId = 999;
            _invoiceServiceMock.Setup(x => x.Get(invoiceId)).ReturnsAsync((Invoice)null);
            var result = await _controller.Delete(invoiceId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_view_when_invoice_exists()
        {
            var invoiceId = 1;
            var invoice = new Invoice { Id = invoiceId, BookingId = 101, Amount = 100.50, Description = "Test Invoice", IsPaid = false };
            _invoiceServiceMock.Setup(x => x.Get(invoiceId)).ReturnsAsync(invoice);
            var result = await _controller.Delete(invoiceId) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Invoice>(result.Model);
            Assert.Equal(invoiceId, model.Id);
        }

        // Edit Tests
        // GET Edit Tests
        [Fact]
        public async Task Edit_should_return_notfound_when_id_is_null()
        {
            int? id = null;
            var result = await _controller.Edit(id);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_should_return_notfound_when_invoice_does_not_exist()
        {
            var invoiceId = 999;
            _invoiceServiceMock.Setup(x => x.Get(invoiceId)).ReturnsAsync((Invoice)null);
            var result = await _controller.Edit(invoiceId) as NotFoundResult;
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Edit_should_return_view_when_invoice_exists()
        {
            var invoiceId = 1;
            var invoice = new Invoice { Id = invoiceId, BookingId = 101, Amount = 100.50, Description = "Test Invoice", IsPaid = false };
            _invoiceServiceMock.Setup(x => x.Get(invoiceId)).ReturnsAsync(invoice);
            var result = await _controller.Edit(invoiceId) as ViewResult;
            Assert.NotNull(result);
            var model = Assert.IsType<Invoice>(result.Model);
            Assert.Equal(invoiceId, model.Id);
        }

        // POST Edit Tests
        [Fact]
        public async Task Edit_post_should_return_notfound_when_id_does_not_match()
        {
            var invoiceId = 1;
            var editedInvoice = new Invoice { Id = 999, BookingId = 101, Amount = 100.50, Description = "Test Invoice", IsPaid = false };

            var result = await _controller.Edit(invoiceId, editedInvoice);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_post_should_redirect_to_index_when_model_is_valid()
        {
            var invoiceId = 1;
            var editedInvoice = new Invoice { Id = invoiceId, BookingId = 101, Amount = 100.50, Description = "Updated Test Invoice", IsPaid = false };

            _invoiceServiceMock.Setup(x => x.Save(It.IsAny<Invoice>())).Returns(Task.CompletedTask);

            var result = await _controller.Edit(invoiceId, editedInvoice) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _invoiceServiceMock.Verify(x => x.Save(It.IsAny<Invoice>()), Times.Once);
        }

        [Fact]
        public async Task Edit_post_should_return_view_when_model_is_invalid()
        {
            var invoiceId = 1;
            var editedInvoice = new Invoice { Id = invoiceId, BookingId = 101, Amount = 100.50, Description = "Test Invoice", IsPaid = false };

            _controller.ModelState.AddModelError("Description", "Description is required");

            var result = await _controller.Edit(invoiceId, editedInvoice) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(editedInvoice, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }
    }
}
