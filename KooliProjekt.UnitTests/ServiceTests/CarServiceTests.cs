using KooliProjekt.Data;
using KooliProjekt.Services;
using KooliProjekt.Models;
using KooliProjekt.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class CarServiceTests : ServiceTestBase
    {
        private readonly ICarService _carService;

        public CarServiceTests()
        {
            _carService = new CarService(DbContext);
        }

        // Test, et auto lisamine töötab ja toob õiged andmed
        [Fact]
        public async Task Save_ShouldAddCar()
        {
            // Arrange
            var car = new Car
            {
                Type = "Sedan",
                RegistrationNumber = "ABC123",
                HourlyRate = 15.50,
                KmRate = 0.25,
                IsAvaliable = true
            };

            // Act
            await _carService.Save(car);

            // Assert
            var createdCar = await DbContext.Cars.FindAsync(car.Id);
            Assert.NotNull(createdCar);
            Assert.Equal("Sedan", createdCar.Type);
            Assert.Equal("ABC123", createdCar.RegistrationNumber);
            Assert.Equal(15.50, createdCar.HourlyRate);
            Assert.Equal(0.25, createdCar.KmRate);
            Assert.True(createdCar.IsAvaliable);
        }

        // Test, et auto saame ID järgi kätte
        [Fact]
        public async Task Get_ShouldReturnCarById()
        {
            // Arrange
            var car = new Car
            {
                Type = "Sedan",
                RegistrationNumber = "ABC123",
                HourlyRate = 15.50,
                KmRate = 0.25,
                IsAvaliable = true
            };
            await _carService.Save(car);

            // Act
            var fetchedCar = await _carService.Get(car.Id);

            // Assert
            Assert.NotNull(fetchedCar);
            Assert.Equal("Sedan", fetchedCar.Type);
            Assert.Equal("ABC123", fetchedCar.RegistrationNumber);
            Assert.Equal(15.50, fetchedCar.HourlyRate);
            Assert.Equal(0.25, fetchedCar.KmRate);
            Assert.True(fetchedCar.IsAvaliable);
        }

        // Test, et auto uuendamine töötab õigesti
        [Fact]
        public async Task Save_ShouldUpdateCar()
        {
            // Arrange
            var car = new Car
            {
                Type = "Sedan",
                RegistrationNumber = "ABC123",
                HourlyRate = 15.50,
                KmRate = 0.25,
                IsAvaliable = true
            };
            await _carService.Save(car);

            car.Type = "SUV";
            car.RegistrationNumber = "XYZ789";
            car.HourlyRate = 20.00;
            car.KmRate = 0.30;
            car.IsAvaliable = false;

            // Act
            await _carService.Save(car);

            // Assert
            var updatedCar = await _carService.Get(car.Id);
            Assert.NotNull(updatedCar);
            Assert.Equal("SUV", updatedCar.Type);
            Assert.Equal("XYZ789", updatedCar.RegistrationNumber);
            Assert.Equal(20.00, updatedCar.HourlyRate);
            Assert.Equal(0.30, updatedCar.KmRate);
            Assert.False(updatedCar.IsAvaliable);
        }

        // Test, et auto eemaldamine töötab õigesti
        [Fact]
        public async Task Delete_ShouldRemoveCar()
        {
            // Arrange
            var car = new Car
            {
                Type = "Sedan",
                RegistrationNumber = "ABC123",
                HourlyRate = 15.50,
                KmRate = 0.25,
                IsAvaliable = true
            };
            await _carService.Save(car);

            // Act
            await _carService.Delete(car.Id);

            // Assert
            var deletedCar = await DbContext.Cars.FindAsync(car.Id);
            Assert.Null(deletedCar);
        }

        // Test, et Includes tagastab õigesti true, kui auto olemas
        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenCarExists()
        {
            // Arrange
            var car = new Car
            {
                Type = "Sedan",
                RegistrationNumber = "ABC123",
                HourlyRate = 15.50,
                KmRate = 0.25,
                IsAvaliable = true
            };
            await _carService.Save(car);

            // Act
            var exists = await _carService.Includes(car.Id);

            // Assert
            Assert.True(exists);
        }

        // Test, et Includes tagastab false, kui autot ei eksisteeri
        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenCarDoesNotExist()
        {
            // Act
            var exists = await _carService.Includes(999); // ID, mida pole olemas

            // Assert
            Assert.False(exists);
        }

        // Test, et lehtede vaatamiseks tagastatakse õiged autod
        [Fact]
        public async Task List_ShouldReturnPagedCars_WhenSearchIsProvided()
        {
            var car1 = new Car { Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 15.50, KmRate = 0.25, IsAvaliable = true };
            var car2 = new Car { Type = "SUV", RegistrationNumber = "XYZ789", HourlyRate = 20.00, KmRate = 0.30, IsAvaliable = false };
            await _carService.Save(car1);
            await _carService.Save(car2);

            var search = new CarSearch { Keyword = "SUV" };

            var result = await _carService.List(1, 10, search);

            Assert.NotNull(result);
            Assert.Single(result.Results);  // Otsing peaks leidma ainult ühe auto
            Assert.Contains(result.Results, c => c.Type == "SUV");
        }

        // Test, et kõik autod tagastatakse, kui otsing on tühi või null
        [Fact]
        public async Task List_ShouldReturnAllCars_WhenSearchIsNull()
        {
            // Arrange
            var car1 = new Car { Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 15.50, KmRate = 0.25, IsAvaliable = true };
            var car2 = new Car { Type = "SUV", RegistrationNumber = "XYZ789", HourlyRate = 20.00, KmRate = 0.30, IsAvaliable = false };
            await _carService.Save(car1);
            await _carService.Save(car2);

            // Act
            var result = await _carService.List(1, 10, null); // Otsing on null

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count); // Kõik autod peaksid olema tagastatud
        }
        [Fact]
        public async Task Search_ShouldReturnCars_WhenIsAvailableIsTrue()
        {
            // Arrange
            var car1 = new Car { Type = "Sedan", RegistrationNumber = "ABC123", IsAvaliable = true };
            var car2 = new Car { Type = "SUV", RegistrationNumber = "XYZ456", IsAvaliable = false };
            await _carService.Save(car1);
            await _carService.Save(car2);

            // Act
            var search = new CarSearch { IsAvaliable = true };
            var result = await _carService.Search(search);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);  // Ainult üks auto on saadaval
            Assert.Contains(result, c => c.IsAvaliable == true);
        }

        [Fact]
        public async Task Search_ShouldReturnCars_WhenIsAvailableIsFalse()
        {
            // Arrange
            var car1 = new Car { Type = "Sedan", RegistrationNumber = "ABC123", IsAvaliable = true };
            var car2 = new Car { Type = "SUV", RegistrationNumber = "XYZ456", IsAvaliable = false };
            await _carService.Save(car1);
            await _carService.Save(car2);

            // Act
            var search = new CarSearch { IsAvaliable = false };
            var result = await _carService.Search(search);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);  // Ainult üks auto ei ole saadaval
            Assert.Contains(result, c => c.IsAvaliable == false);
        }

        [Fact]
        public async Task Search_ShouldReturnAllCars_WhenIsAvailableIsNull()
        {
            // Arrange
            var car1 = new Car { Type = "Sedan", RegistrationNumber = "ABC123", IsAvaliable = true };
            var car2 = new Car { Type = "SUV", RegistrationNumber = "XYZ456", IsAvaliable = false };
            await _carService.Save(car1);
            await _carService.Save(car2);

            // Act
            var search = new CarSearch { IsAvaliable = null };  // Kui IsAvailable on null, tagastatakse kõik autod
            var result = await _carService.Search(search);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Kõik autod peaksid olema tagastatud
        }


    }
}
