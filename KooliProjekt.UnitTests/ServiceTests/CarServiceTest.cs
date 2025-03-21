using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using KooliProjekt.Services;
using Moq;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class CarServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ICarRepository> _repositoryMock;
        private readonly CarService _carService;

        public CarServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _repositoryMock = new Mock<ICarRepository>();
            _carService = new CarService(_uowMock.Object);

            _uowMock.SetupGet(r => r.CarRepository)
                    .Returns(_repositoryMock.Object);
        }

        [Fact]
        public async Task List_should_return_list_of_cars()
        {
            // Arrange
            var results = new List<Car>
            {
                new Car { Id = 1 },
                new Car { Id = 2 }
            };
            var pagedResult = new PagedResult<Car> { Results = results };
            _repositoryMock.Setup(r => r.List(It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(pagedResult);

            // Act
            var result = await _carService.List(1, 10);

            // Assert
            Assert.Equal(pagedResult, result);
        }

        [Fact]
        public async Task Delete_should_call_delete_on_repository()
        {
            // Arrange
            var carId = 1;

            // Act
            await _carService.Delete(carId);

            // Assert
            _repositoryMock.Verify(r => r.Delete(carId), Times.Once);
        }

        [Fact]
        public async Task Get_should_return_car_from_repository()
        {
            // Arrange
            var carId = 1;
            var expectedCar = new Car { Id = carId };
            _repositoryMock.Setup(r => r.Get(carId)).ReturnsAsync(expectedCar);

            // Act
            var result = await _carService.Get(carId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(carId, result.Id);
        }

        [Fact]
        public async Task Save_should_call_save_on_repository()
        {
            // Arrange
            var car = new Car { Id = 1 };

            // Act
            await _carService.Save(car);

            // Assert
            _repositoryMock.Verify(r => r.Save(car), Times.Once);
        }
    }
}
