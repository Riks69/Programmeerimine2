using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using KooliProjekt.WpfApp;
using KooliProjekt.WpfApp.Api;
using KooliProjekt.WpfApp.ApiAdd;
using Moq;
using Xunit;

namespace KooliProjekt.WpfApp.UnitTests
{
    public class MainWindowViewModelTests
    {
        [Fact]
        public async Task Load_ShouldPopulateLists_WhenApiReturnsCustomers()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "John Doe", Password = "password123", IsRegistered = true },
                new Customer { Id = 2, Name = "Jane Smith", Password = "password456", IsRegistered = true }
            };

            var result = new Result<List<Customer>> { Value = customers };
            mockApiClient.Setup(api => api.List()).ReturnsAsync(result);

            var viewModel = new MainWindowViewModel(mockApiClient.Object);

            // Act
            await viewModel.LoadCustomers();
            var lists = (ObservableCollection<Customer>)viewModel.Lists;

            // Assert
            Assert.Equal(2, lists.Count);
        Assert.Equal("John Doe", lists[0].Name);  // Correct usage
            Assert.Equal("Jane Smith", lists[1].Name);
        }

        [Fact]
        public async Task Load_ShouldNotThrow_WhenApiReturnsNull()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var result = new Result<List<Customer>> { Value = null, Error = "Some error" };
            mockApiClient.Setup(api => api.List()).ReturnsAsync(result);  // Tagastab Result, mille väärtus on null

            var viewModel = new MainWindowViewModel(mockApiClient.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => viewModel.LoadCustomers());  // Kasutage Load meetodit, mitte LoadCustomers
            var lists = viewModel.Lists;  // Sellel peaks olema õiged andmed (ObservableCollection<Customer>)

            // Assert
            Assert.Null(exception);  // Veenduge, et ei viskaks erandit
            Assert.Empty((string)lists);  // Kui API tagastab null, siis peaks list tühi olema
        }


        [Fact]
        public async Task SaveCommand_ShouldSaveCustomer_WhenValidCustomerIsSelected()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Password = "password123",
                Email = "john.doe@example.com",
                IsRegistered = true
            };

            var viewModel = new MainWindowViewModel(mockApiClient.Object);
            viewModel.SelectedCustomer  = customer;

            mockApiClient.Setup(api => api.Save(It.IsAny<Customer>())).Returns(Task.FromResult(Result.Success()));

            // Act
            viewModel.SaveCommand.Execute(customer);

            // Assert
            mockApiClient.Verify(api => api.Save(It.Is<Customer>(c => c == customer)), Times.Once);
        }

        [Fact]
        public async Task DeleteCommand_ShouldDeleteCustomer_WhenSelectedItemIsValid()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Password = "password123",
                Email = "john.doe@example.com",
                IsRegistered = true
            };

            var viewModel = new MainWindowViewModel(mockApiClient.Object);
            viewModel.SelectedCustomer = customer;

            mockApiClient.Setup(api => api.Delete(It.IsAny<int>())).Returns(Task.FromResult(Result.Success()));  // Tagastab edu Result

            // Act
            viewModel.DeleteCommand.Execute(customer);

            // Assert
            mockApiClient.Verify(api => api.Delete(It.Is<int>(id => id == customer.Id)), Times.Once);
            Assert.Null(viewModel.SelectedCustomer); // SelectedItem should be null after deletion
        }

        [Fact]
        public async Task DeleteCommand_ShouldNotDeleteCustomer_WhenConfirmDeleteReturnsFalse()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var customer = new Customer
            {
                Id = 1,
                Name = "John Doe",
                Password = "password123",
                Email = "john.doe@example.com",
                IsRegistered = true
            };

            var viewModel = new MainWindowViewModel(mockApiClient.Object);
            viewModel.SelectedCustomer = customer;
            viewModel.ConfirmDelete = c => false; // Simulate user canceling the delete action

            // Act
            viewModel.DeleteCommand.Execute(customer);

            // Assert
            mockApiClient.Verify(api => api.Delete(It.IsAny<int>()), Times.Never); // Delete should not be called
            Assert.Equal(customer, viewModel.SelectedCustomer); // SelectedItem should remain the same
        }
    }
}
