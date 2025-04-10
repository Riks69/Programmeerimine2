using KooliProjekt.Data;
using KooliProjekt.Services;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using KooliProjekt.Search;

namespace KooliProjekt.Tests
{
    public class CarServiceTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly CarService _carService;

        public CarServiceTests()
        {
            // Loome mockitud ApplicationDbContext-i
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            var context = new ApplicationDbContext(options);
            _carService = new CarService(context);
        }

        [Fact]
        public async Task Save_ShouldAddNewCar_WhenIdIsZero()
        {
            // Arrange
            var newCar = new Car
            {
                Id = 0,
                Type = "Sedan",
                RegistrationNumber = "XYZ123",
                HourlyRate = 20.0,
                KmRate = 0.1,
                IsAvaliable = true
            };

            // Act
            await _carService.Save(newCar);

            // Assert
            var carFromDb = await _carService.Get(newCar.Id);
            Assert.NotNull(carFromDb);
            Assert.Equal(newCar.Type, carFromDb.Type);
        }

        [Fact]
        public async Task Save_ShouldUpdateCar_WhenIdIsNotZero()
        {
            // Arrange
            var existingCar = new Car
            {
                Id = 1,
                Type = "Sedan",
                RegistrationNumber = "XYZ123",
                HourlyRate = 20.0,
                KmRate = 0.1,
                IsAvaliable = true
            };
            await _carService.Save(existingCar);

            var updatedCar = new Car
            {
                Id = 1,
                Type = "SUV",
                RegistrationNumber = "ABC456",
                HourlyRate = 25.0,
                KmRate = 0.12,
                IsAvaliable = false
            };

            // Act
            await _carService.Save(updatedCar);

            // Assert
            var carFromDb = await _carService.Get(updatedCar.Id);
            Assert.Equal(updatedCar.Type, carFromDb.Type);
            Assert.Equal(updatedCar.RegistrationNumber, carFromDb.RegistrationNumber);
            Assert.Equal(updatedCar.HourlyRate, carFromDb.HourlyRate);
            Assert.Equal(updatedCar.KmRate, carFromDb.KmRate);
            Assert.Equal(updatedCar.IsAvaliable, carFromDb.IsAvaliable);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCar_WhenCarExists()
        {
            // Arrange
            var car = new Car
            {
                Id = 1,
                Type = "Sedan",
                RegistrationNumber = "XYZ123",
                HourlyRate = 20.0,
                KmRate = 0.1,
                IsAvaliable = true
            };
            await _carService.Save(car);

            // Act
            await _carService.Delete(car.Id);

            // Assert
            var carFromDb = await _carService.Get(car.Id);
            Assert.Null(carFromDb);
        }

        [Fact]
        public async Task Get_ShouldReturnCar_WhenCarExists()
        {
            // Arrange
            var car = new Car
            {
                Id = 1,
                Type = "Sedan",
                RegistrationNumber = "XYZ123",
                HourlyRate = 20.0,
                KmRate = 0.1,
                IsAvaliable = true
            };
            await _carService.Save(car);

            // Act
            var carFromDb = await _carService.Get(car.Id);

            // Assert
            Assert.NotNull(carFromDb);
            Assert.Equal(car.Type, carFromDb.Type);
            Assert.Equal(car.RegistrationNumber, carFromDb.RegistrationNumber);
        }

        [Fact]
        public async Task List_ShouldReturnPagedResults()
        {
            // Arrange
            var car1 = new Car { Id = 1, Type = "Sedan", RegistrationNumber = "XYZ123", HourlyRate = 20.0, KmRate = 0.1, IsAvaliable = true };
            var car2 = new Car { Id = 2, Type = "SUV", RegistrationNumber = "ABC456", HourlyRate = 25.0, KmRate = 0.12, IsAvaliable = false };
            var car3 = new Car { Id = 3, Type = "Hatchback", RegistrationNumber = "DEF789", HourlyRate = 15.0, KmRate = 0.08, IsAvaliable = true };

            await _carService.Save(car1);
            await _carService.Save(car2);
            await _carService.Save(car3);

            // Act
            var pagedCars = await _carService.List(1, 2);

            // Assert
            Assert.Equal(2, pagedCars.Items.Count); // We expect two cars in the first page
        }

        [Fact]
        public async Task Search_ShouldReturnCars_WhenSearchIsUsed()
        {
            // Arrange
            var car1 = new Car { Id = 1, Type = "Sedan", RegistrationNumber = "XYZ123", HourlyRate = 20.0, KmRate = 0.1, IsAvaliable = true };
            var car2 = new Car { Id = 2, Type = "SUV", RegistrationNumber = "ABC456", HourlyRate = 25.0, KmRate = 0.12, IsAvaliable = false };

            await _carService.Save(car1);
            await _carService.Save(car2);

            var search = new CarSearch { Keyword = "SUV" };

            // Act
            var results = await _carService.Search(search);

            // Assert
            Assert.Single(results); // Only one result should be returned (car2)
            Assert.Equal(car2.Type, results[0].Type);
        }

        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenCarExists()
        {
            // Arrange
            var car = new Car { Id = 1, Type = "Sedan", RegistrationNumber = "XYZ123", HourlyRate = 20.0, KmRate = 0.1, IsAvaliable = true };
            await _carService.Save(car);

            // Act
            var exists = await _carService.Includes(car.Id);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenCarDoesNotExist()
        {
            // Arrange & Act
            var exists = await _carService.Includes(999); // Non-existing car ID

            // Assert
            Assert.False(exists);
        }
    }
}
