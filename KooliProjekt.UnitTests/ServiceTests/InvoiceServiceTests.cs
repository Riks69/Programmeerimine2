using KooliProjekt.Data;
using KooliProjekt.Models;
using KooliProjekt.Search;
using KooliProjekt.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class InvoiceServiceTests : ServiceTestBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceServiceTests()
        {
            _invoiceService = new InvoiceService(DbContext);
        }

        [Fact]
        public async Task Save_ShouldAddInvoice_WhenIdIsZero()
        {
            var invoice = new Invoice
            {
                BookingId = 1,
                Amount = 100,
                Description = "Test Invoice",
                IsPaid = false
            };

            await _invoiceService.Save(invoice);
            var saved = await _invoiceService.Get(invoice.Id);

            Assert.NotNull(saved);
            Assert.Equal("Test Invoice", saved.Description);
        }

        [Fact]
        public async Task Save_ShouldUpdateInvoice_WhenExists()
        {
            var invoice = new Invoice
            {
                BookingId = 2,
                Amount = 200,
                Description = "Old Desc",
                IsPaid = false
            };

            await _invoiceService.Save(invoice);
            invoice.Description = "Updated Desc";
            invoice.IsPaid = true;

            await _invoiceService.Save(invoice);
            var updated = await _invoiceService.Get(invoice.Id);

            Assert.Equal("Updated Desc", updated.Description);
            Assert.True(updated.IsPaid);
        }

        [Fact]
        public async Task Save_ShouldDoNothing_WhenInvoiceDoesNotExist()
        {
            var invoice = new Invoice
            {
                Id = 9999,  // Eeldame, et see ID ei eksisteeri andmebaasis
                BookingId = 10,
                Amount = 50,
                Description = "Non-existent Invoice",
                IsPaid = false
            };

            // Ei tehta midagi, kuna see arve ei eksisteeri (ID on vale)
            await _invoiceService.Save(invoice);

            // Kontrollime, kas arve ei ole salvestatud (ei peaks leidma)
            var result = await _invoiceService.Get(invoice.Id);
            Assert.Null(result);  // Arve ei tohiks eksisteerida
        }


        [Fact]
        public async Task Delete_ShouldRemoveInvoice()
        {
            var invoice = new Invoice
            {
                BookingId = 3,
                Amount = 300,
                Description = "To Delete",
                IsPaid = false
            };

            await _invoiceService.Save(invoice);
            await _invoiceService.Delete(invoice.Id);

            var deleted = await _invoiceService.Get(invoice.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Get_ShouldReturnInvoice_WhenExists()
        {
            var invoice = new Invoice
            {
                BookingId = 4,
                Amount = 400,
                Description = "GetTest",
                IsPaid = false
            };

            await _invoiceService.Save(invoice);
            var result = await _invoiceService.Get(invoice.Id);

            Assert.NotNull(result);
            Assert.Equal("GetTest", result.Description);
        }

        [Fact]
        public async Task Includes_ShouldReturnTrue_IfInvoiceExists()
        {
            var invoice = new Invoice
            {
                BookingId = 5,
                Amount = 500,
                Description = "IncludesTest",
                IsPaid = true
            };

            await _invoiceService.Save(invoice);
            var exists = await _invoiceService.Includes(invoice.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task Includes_ShouldReturnFalse_IfInvoiceDoesNotExist()
        {
            var exists = await _invoiceService.Includes(999);
            Assert.False(exists);
        }

        [Fact]
        public async Task List_ShouldReturnPagedResult()
        {
            await _invoiceService.Save(new Invoice { BookingId = 10, Amount = 150, Description = "First", IsPaid = true });
            await _invoiceService.Save(new Invoice { BookingId = 11, Amount = 200, Description = "Second", IsPaid = false });

            var result = await _invoiceService.List(1, 10, null);

            Assert.NotNull(result);
            Assert.True(result.Results.Count >= 2);
        }

        [Fact]
        public async Task List_ShouldFilterByKeyword()
        {
            await _invoiceService.Save(new Invoice { BookingId = 123, Amount = 99.99, Description = "Special Invoice", IsPaid = false });
            await _invoiceService.Save(new Invoice { BookingId = 456, Amount = 19.99, Description = "Generic", IsPaid = true });

            var result = await _invoiceService.List(1, 10, new InvoiceSearch { Keyword = "special" });

            Assert.Single(result.Results);
            Assert.Contains(result.Results, i => i.Description.Contains("Special"));
        }

        [Fact]
        public async Task Search_ShouldReturnInvoices_ByKeyword()
        {
            await _invoiceService.Save(new Invoice { BookingId = 10, Amount = 80, Description = "SearchTest", IsPaid = false });
            await _invoiceService.Save(new Invoice { BookingId = 20, Amount = 180, Description = "Other", IsPaid = true });

            var result = await _invoiceService.Search(new InvoiceSearch { Keyword = "search" });

            Assert.Single(result);
            Assert.Contains(result, i => i.Description.Contains("SearchTest"));
        }

        [Fact]
        public async Task Search_ShouldReturnInvoices_ByIsPaidTrue()
        {
            await _invoiceService.Save(new Invoice { BookingId = 10, Amount = 99, Description = "Paid", IsPaid = true });
            await _invoiceService.Save(new Invoice { BookingId = 11, Amount = 55, Description = "Unpaid", IsPaid = false });

            var result = await _invoiceService.Search(new InvoiceSearch { Done = true });

            Assert.Single(result);
            Assert.All(result, i => Assert.True(i.IsPaid));
        }

        [Fact]
        public async Task Search_ShouldReturnInvoices_ByIsPaidFalse()
        {
            await _invoiceService.Save(new Invoice { BookingId = 10, Amount = 99, Description = "Paid", IsPaid = true });
            await _invoiceService.Save(new Invoice { BookingId = 11, Amount = 55, Description = "Unpaid", IsPaid = false });

            var result = await _invoiceService.Search(new InvoiceSearch { Done = false });

            Assert.Single(result);
            Assert.All(result, i => Assert.False(i.IsPaid));
        }

        [Fact]
        public async Task Search_ShouldReturnAll_WhenSearchIsNull()
        {
            await _invoiceService.Save(new Invoice { BookingId = 100, Amount = 10, Description = "NullTest1", IsPaid = true });
            await _invoiceService.Save(new Invoice { BookingId = 101, Amount = 20, Description = "NullTest2", IsPaid = false });

            var result = await _invoiceService.Search(null);

            Assert.True(result.Count >= 2);
        }
    }
}
