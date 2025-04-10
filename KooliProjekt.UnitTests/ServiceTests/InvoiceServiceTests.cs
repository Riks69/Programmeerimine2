using KooliProjekt.Data;
using KooliProjekt.Services;
using KooliProjekt.Models;
using KooliProjekt.Search;
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
            _invoiceService = new InvoiceService(DbContext);  // Kutsume üles InvoiceService testimiseks
        }

        // Create
        [Fact]
        public async Task Create_ShouldAddInvoice()
        {
            var invoice = new Invoice
            {
                BookingId = 1,
                Amount = 150,
                Description = "Test invoice",
                Id = 0  // ID 0 tähendab, et see on uus arve
            };

            await _invoiceService.Save(invoice);

            var createdInvoice = await DbContext.Invoices.FindAsync(invoice.Id);
            Assert.NotNull(createdInvoice);
            Assert.Equal(1, createdInvoice.BookingId);
            Assert.Equal(150, createdInvoice.Amount);
            Assert.Equal("Test invoice", createdInvoice.Description);
            Assert.True(createdInvoice.Id > 0);  // Kontrollime, et ID on määratud pärast salvestamist
        }

        // Delete
        [Fact]
        public async Task Delete_ShouldRemoveInvoice()
        {
            var invoice = new Invoice
            {
                BookingId = 1,
                Amount = 150,
                Description = "Test invoice"
            };
            await _invoiceService.Save(invoice);

            await _invoiceService.Delete(invoice.Id);

            var deletedInvoice = await DbContext.Invoices.FindAsync(invoice.Id);
            Assert.Null(deletedInvoice);  // Kontrollime, et arve on andmebaasist kustutatud
        }

        // Get
        [Fact]
        public async Task Get_ShouldReturnInvoiceById()
        {
            var invoice = new Invoice
            {
                BookingId = 1,
                Amount = 150,
                Description = "Test invoice"
            };
            await _invoiceService.Save(invoice);

            var retrievedInvoice = await _invoiceService.Get(invoice.Id);
            Assert.NotNull(retrievedInvoice);
            Assert.Equal(invoice.BookingId, retrievedInvoice.BookingId);
            Assert.Equal(invoice.Amount, retrievedInvoice.Amount);
            Assert.Equal(invoice.Description, retrievedInvoice.Description);
        }

        // Includes
        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenInvoiceDoesNotExist()
        {
            var exists = await _invoiceService.Includes(999);  // Arve ID, mida pole olemas

            Assert.False(exists);  // Kontrollime, et arve ei eksisteeri
        }

        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenInvoiceExists()
        {
            var invoice = new Invoice
            {
                BookingId = 1,
                Amount = 150,
                Description = "Test invoice"
            };
            await _invoiceService.Save(invoice);

            var exists = await _invoiceService.Includes(invoice.Id);

            Assert.True(exists);  // Kontrollime, et arve eksisteerib
        }

        // List
        [Fact]
        public async Task List_ShouldReturnAllInvoices_WhenSearchIsNull()
        {
            var invoice1 = new Invoice { BookingId = 1, Amount = 150, Description = "Invoice for Booking 1" };
            var invoice2 = new Invoice { BookingId = 2, Amount = 200, Description = "Invoice for Booking 2" };
            await _invoiceService.Save(invoice1);
            await _invoiceService.Save(invoice2);

            var result = await _invoiceService.List(1, 10, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);  // Kõik arved peaksid olema tagastatud
        }

        [Fact]
        public async Task List_ShouldReturnPagedInvoices_WhenKeywordMatches()
        {
            var invoice1 = new Invoice { BookingId = 1, Amount = 150, Description = "Invoice for Booking 1" };
            var invoice2 = new Invoice { BookingId = 2, Amount = 200, Description = "Invoice for Booking 2" };
            await _invoiceService.Save(invoice1);
            await _invoiceService.Save(invoice2);

            var search = new InvoiceSearch { Keyword = "Booking 1" };

            var result = await _invoiceService.List(1, 10, search);

            Assert.NotNull(result);
            Assert.Single(result.Results);
            Assert.Contains(result.Results, i => i.Description.Contains("Booking 1"));
        }

        // Save
        [Fact]
        public async Task Save_ShouldNotUpdateInvoice_WhenInvoiceDoesNotExist()
        {
            var invoice = new Invoice { BookingId = 1, Amount = 150, Description = "New invoice" };
            await _invoiceService.Save(invoice);  // Lisame uue arve

            // Kontrollime, et arve lisati
            var createdInvoice = await DbContext.Invoices.FindAsync(invoice.Id);
            Assert.NotNull(createdInvoice);
        }

        [Fact]
        public async Task Save_ShouldUpdateInvoice_WhenInvoiceExists()
        {
            var invoice = new Invoice { BookingId = 1, Amount = 150, Description = "Test invoice" };
            await _invoiceService.Save(invoice);

            // Uuendame arve andmed
            invoice.Amount = 200;
            invoice.Description = "Updated invoice description";

            await _invoiceService.Save(invoice);  // Salvestame uuendatud arve

            var updatedInvoice = await _invoiceService.Get(invoice.Id);
            Assert.NotNull(updatedInvoice);
            Assert.Equal(200, updatedInvoice.Amount);
            Assert.Equal("Updated invoice description", updatedInvoice.Description);
        }

        // Search
        [Fact]
        public async Task Search_ShouldReturnInvoices_WhenKeywordIsEmpty()
        {
            var invoice1 = new Invoice { BookingId = 1, Amount = 150, Description = "Invoice for Booking 1" };
            var invoice2 = new Invoice { BookingId = 2, Amount = 200, Description = "Invoice for Booking 2" };
            await _invoiceService.Save(invoice1);
            await _invoiceService.Save(invoice2);

            var search = new InvoiceSearch { Keyword = "" };

            var result = await _invoiceService.List(1, 10, search);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);  // Kõik arved peaksid olema tagastatud
        }

        [Fact]
        public async Task Search_ShouldReturnInvoices_WhenSearchIsNull()
        {
            var invoice1 = new Invoice { BookingId = 1, Amount = 150, Description = "Invoice for Booking 1" };
            var invoice2 = new Invoice { BookingId = 2, Amount = 200, Description = "Invoice for Booking 2" };
            await _invoiceService.Save(invoice1);
            await _invoiceService.Save(invoice2);

            var result = await _invoiceService.List(1, 10, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);  // Kõik arved peaksid olema tagastatud
        }

        [Fact]
        public async Task Search_ShouldReturnInvoices_WhenKeywordMatches()
        {
            var invoice1 = new Invoice { BookingId = 1, Amount = 150, Description = "Invoice for Booking 1" };
            var invoice2 = new Invoice { BookingId = 2, Amount = 200, Description = "Invoice for Booking 2" };
            await _invoiceService.Save(invoice1);
            await _invoiceService.Save(invoice2);

            var search = new InvoiceSearch { Keyword = "Booking 1" };

            var result = await _invoiceService.List(1, 10, search);

            Assert.NotNull(result);
            Assert.Single(result.Results);  // Tagastatakse ainult üks arve, mis vastab otsingule
        }

        [Fact]
        public async Task Search_ShouldReturnInvoices_WhenKeywordAndDoneStatusMatch()
        {
            var invoice1 = new Invoice { BookingId = 1, Amount = 150, Description = "Invoice for Booking 1" };
            var invoice2 = new Invoice { BookingId = 2, Amount = 200, Description = "Invoice for Booking 2" };
            await _invoiceService.Save(invoice1);
            await _invoiceService.Save(invoice2);

            var search = new InvoiceSearch { Keyword = "Invoice for Booking 1" };

            var result = await _invoiceService.List(1, 10, search);

            Assert.NotNull(result);
            Assert.Contains(result.Results, i => i.Description.Contains("Booking 1"));
        }
    }
}
