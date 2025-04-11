using KooliProjekt.Data;
using KooliProjekt.Models;
using KooliProjekt.Search;
using KooliProjekt.Services;
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

        [Fact]
        public async Task Save_ShouldAddNewCar()
        {
            var car = new Car
            {
                Type = "Sedan",
                RegistrationNumber = "123ABC",
                HourlyRate = 10,
                KmRate = 0.5,
                IsAvaliable = true
            };

            await _carService.Save(car);

            var created = await DbContext.Cars.FindAsync(car.Id);
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal("Sedan", created.Type);
        }

        [Fact]
        public async Task Save_ShouldUpdateCar_WhenCarExists()
        {
            var car = new Car
            {
                Type = "SUV",
                RegistrationNumber = "XYZ789",
                HourlyRate = 15,
                KmRate = 0.6,
                IsAvaliable = true
            };
            await _carService.Save(car);

            car.Type = "Van";
            car.HourlyRate = 20;
            await _carService.Save(car);

            var updated = await _carService.Get(car.Id);
            Assert.Equal("Van", updated.Type);
            Assert.Equal(20, updated.HourlyRate);
        }

        [Fact]
        public async Task Save_ShouldDoNothing_WhenCarDoesNotExist()
        {
            var car = new Car
            {
                Id = 999, // Ei eksisteeri
                Type = "Sedan",
                RegistrationNumber = "123ABC",
                HourlyRate = 10,
                KmRate = 0.5,
                IsAvaliable = true
            };

            var existing = await _carService.Get(car.Id);
            Assert.Null(existing);

            await _carService.Save(car); // Ei tohiks midagi teha

            var check = await _carService.Get(car.Id);
            Assert.Null(check);
        }

        [Fact]
        public async Task Get_ShouldReturnCar_WhenExists()
        {
            var car = new Car
            {
                Type = "Hatchback",
                RegistrationNumber = "ABC123",
                HourlyRate = 12,
                KmRate = 0.4,
                IsAvaliable = false
            };
            await _carService.Save(car);

            var result = await _carService.Get(car.Id);

            Assert.NotNull(result);
            Assert.Equal("Hatchback", result.Type);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCar()
        {
            var car = new Car
            {
                Type = "Truck",
                RegistrationNumber = "TRK321",
                HourlyRate = 25,
                KmRate = 1.2,
                IsAvaliable = true
            };
            await _carService.Save(car);

            await _carService.Delete(car.Id);

            var deleted = await _carService.Get(car.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Includes_ShouldReturnTrue_WhenCarExists()
        {
            var car = new Car
            {
                Type = "Minivan",
                RegistrationNumber = "MINI123",
                HourlyRate = 13,
                KmRate = 0.7,
                IsAvaliable = true
            };
            await _carService.Save(car);

            var exists = await _carService.Includes(car.Id);

            Assert.True(exists);
        }

        [Fact]
        public async Task Includes_ShouldReturnFalse_WhenCarDoesNotExist()
        {
            var exists = await _carService.Includes(999);
            Assert.False(exists);
        }

        [Fact]
        public async Task List_ShouldReturnPagedCars_WhenSearchIsNull()
        {
            await _carService.Save(new Car { Type = "Sedan", RegistrationNumber = "1", HourlyRate = 10, KmRate = 0.5, IsAvaliable = true });
            await _carService.Save(new Car { Type = "SUV", RegistrationNumber = "2", HourlyRate = 15, KmRate = 0.6, IsAvaliable = false });

            var result = await _carService.List(1, 10, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Results.Count);
        }

        [Fact]
        public async Task List_ShouldFilterByKeyword()
        {
            await _carService.Save(new Car { Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 10, KmRate = 0.5, IsAvaliable = true });
            await _carService.Save(new Car { Type = "SUV", RegistrationNumber = "XYZ789", HourlyRate = 20, KmRate = 0.8, IsAvaliable = false });

            var search = new CarSearch { Keyword = "SUV" };
            var result = await _carService.List(1, 10, search);

            Assert.Single(result.Results);
            Assert.Equal("SUV", result.Results[0].Type);
        }

        [Fact]
        public async Task Search_ShouldReturnMatchingCars_ByKeywordAndDone()
        {
            await _carService.Save(new Car { Type = "Truck", RegistrationNumber = "TRUCK1", HourlyRate = 30, KmRate = 1.0, IsAvaliable = true });
            await _carService.Save(new Car { Type = "Truck", RegistrationNumber = "TRUCK2", HourlyRate = 35, KmRate = 1.2, IsAvaliable = false });

            var search = new CarSearch { Keyword = "Truck", Done = true };

            var result = await _carService.Search(search);

            Assert.Single(result);
            Assert.True(result[0].IsAvaliable);
        }

        [Fact]
        public async Task Search_ShouldReturnAll_WhenSearchIsNull()
        {
            await _carService.Save(new Car { Type = "Sedan", RegistrationNumber = "1", HourlyRate = 10, KmRate = 0.5, IsAvaliable = true });
            await _carService.Save(new Car { Type = "SUV", RegistrationNumber = "2", HourlyRate = 15, KmRate = 0.6, IsAvaliable = false });

            var result = await _carService.Search(null);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Search_ShouldReturnAvailableCars_WhenDoneIsTrue()
        {
            await _carService.Save(new Car { Type = "Van", RegistrationNumber = "VAN1", HourlyRate = 12, KmRate = 0.7, IsAvaliable = true });
            await _carService.Save(new Car { Type = "Van", RegistrationNumber = "VAN2", HourlyRate = 12, KmRate = 0.7, IsAvaliable = false });

            var search = new CarSearch { Done = true };
            var result = await _carService.Search(search);

            Assert.Single(result);
            Assert.True(result[0].IsAvaliable);
        }

        [Fact]
        public async Task Search_ShouldReturnUnavailableCars_WhenDoneIsFalse()
        {
            await _carService.Save(new Car { Type = "Mini", RegistrationNumber = "MINI1", HourlyRate = 8, KmRate = 0.3, IsAvaliable = true });
            await _carService.Save(new Car { Type = "Mini", RegistrationNumber = "MINI2", HourlyRate = 8, KmRate = 0.3, IsAvaliable = false });

            var search = new CarSearch { Done = false };
            var result = await _carService.Search(search);

            Assert.Single(result);
            Assert.False(result[0].IsAvaliable);
        }
    }
}
