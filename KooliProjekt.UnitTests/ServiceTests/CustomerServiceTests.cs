using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using KooliProjekt.Services;
using Moq;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class CustomerServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _repositoryMock = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_uowMock.Object);

            _uowMock.SetupGet(r => r.CustomerRepository)
                    .Returns(_repositoryMock.Object);
        }

        [Fact]
        public async Task List_should_return_list_of_customers()
        {
            // Arrange
            var results = new List<Customer>
            {
                new Customer { Id = 1 },
                new Customer { Id = 2 }
            };
            var pagedResult = new PagedResult<Customer> { Results = results };
            _repositoryMock.Setup(r => r.List(It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(pagedResult);

            // Act
            var result = await _customerService.List(1, 10);

            // Assert
            Assert.Equal(pagedResult, result);
        }

        [Fact]
        public async Task Delete_should_call_delete_on_repository()
        {
            // Arrange
            var customerId = 1;

            // Act
            await _customerService.Delete(customerId);

            // Assert
            _repositoryMock.Verify(r => r.Delete(customerId), Times.Once);
        }

        [Fact]
        public async Task Get_should_return_customer_from_repository()
        {
            // Arrange
            var customerId = 1;
            var expectedCustomer = new Customer { Id = customerId };
            _repositoryMock.Setup(r => r.Get(customerId)).ReturnsAsync(expectedCustomer);

            // Act
            var result = await _customerService.Get(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.Id);
        }

        [Fact]
        public async Task Save_should_call_save_on_repository()
        {
            // Arrange
            var customer = new Customer { Id = 1 };

            // Act
            await _customerService.Save(customer);

            // Assert
            _repositoryMock.Verify(r => r.Save(customer), Times.Once);
        }
    }
}
