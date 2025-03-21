using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using KooliProjekt.Services;
using Moq;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IInvoiceRepository> _repositoryMock;
        private readonly InvoiceService _invoiceService;

        public InvoiceServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _repositoryMock = new Mock<IInvoiceRepository>();
            _invoiceService = new InvoiceService(_uowMock.Object);

            _uowMock.SetupGet(r => r.InvoiceRepository)
                    .Returns(_repositoryMock.Object);
        }

        [Fact]
        public async Task List_should_return_list_of_invoices()
        {
            // Arrange
            var results = new List<Invoice>
            {
                new Invoice { Id = 1 },
                new Invoice { Id = 2 }
            };
            var pagedResult = new PagedResult<Invoice> { Results = results };
            _repositoryMock.Setup(r => r.List(It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(pagedResult);

            // Act
            var result = await _invoiceService.List(1, 10);

            // Assert
            Assert.Equal(pagedResult, result);
        }

        [Fact]
        public async Task Delete_should_call_delete_on_repository()
        {
            // Arrange
            var invoiceId = 1;

            // Act
            await _invoiceService.Delete(invoiceId);

            // Assert
            _repositoryMock.Verify(r => r.Delete(invoiceId), Times.Once);
        }

        [Fact]
        public async Task Get_should_return_invoice_from_repository()
        {
            // Arrange
            var invoiceId = 1;
            var expectedInvoice = new Invoice { Id = invoiceId };
            _repositoryMock.Setup(r => r.Get(invoiceId)).ReturnsAsync(expectedInvoice);

            // Act
            var result = await _invoiceService.Get(invoiceId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(invoiceId, result.Id);
        }

        [Fact]
        public async Task Save_should_call_save_on_repository()
        {
            // Arrange
            var invoice = new Invoice { Id = 1 };

            // Act
            await _invoiceService.Save(invoice);

            // Assert
            _repositoryMock.Verify(r => r.Save(invoice), Times.Once);
        }
    }
}
