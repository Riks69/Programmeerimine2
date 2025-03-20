using KooliProjekt.Controllers;
using KooliProjekt.Data;
using KooliProjekt.Models;
using KooliProjekt.Search;
using KooliProjekt.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ControllerTests
{
    public class CarsControllerTests
    {
        private readonly CarsController _controller;
        private readonly Mock<ICarService> _carServiceMock;

        public CarsControllerTests()
        {
            _carServiceMock = new Mock<ICarService>();
            _controller = new CarsController(_carServiceMock.Object);
        }

        // Index Tests

        [Fact]
        public async Task Index_should_return_view_with_cars()
        {
            // Arrange
            var cars = new PagedResult<Car>
            {
                Results = new List<Car>
                {
                    new Car { Id = 1, Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true },
                    new Car { Id = 2, Type = "SUV", RegistrationNumber = "XYZ456", HourlyRate = 30, KmRate = 0.3, IsAvaliable = true }
                }
            };
            _carServiceMock.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CarSearch>())).ReturnsAsync(cars);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<CarIndexModel>(result.Model);
            Assert.Equal(cars.Results.Count, model.Data.Results.Count);
        }

        // Details Tests

        [Fact]
        public async Task Details_should_return_notfound_when_id_is_null()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_should_return_notfound_when_car_not_found()
        {
            // Arrange
            var carId = 999;
            _carServiceMock.Setup(x => x.Get(carId)).ReturnsAsync((Car)null);

            // Act
            var result = await _controller.Details(carId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_should_return_view_when_car_exists()
        {
            // Arrange
            var carId = 1;
            var car = new Car { Id = carId, Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true };
            _carServiceMock.Setup(x => x.Get(carId)).ReturnsAsync(car);

            // Act
            var result = await _controller.Details(carId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Car>(result.Model);
            Assert.Equal(carId, model.Id);
        }

        // Create Tests

        [Fact]
        public void Create_should_return_view()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);  // Veenduge, et tulemus ei ole null
        }

        [Fact]
        public async Task Create_should_return_view_when_model_is_invalid()
        {
            // Arrange
            var car = new Car { Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true };
            _controller.ModelState.AddModelError("RegistrationNumber", "Registration number is required");

            // Act
            var result = await _controller.Create(car) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(car, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_should_redirect_to_index_when_model_is_valid()
        {
            // Arrange
            var car = new Car { Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true };
            _carServiceMock.Setup(x => x.Save(It.IsAny<Car>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(car) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _carServiceMock.Verify(x => x.Save(It.IsAny<Car>()), Times.Once);
        }

        // Edit Tests

        [Fact]
        public async Task Edit_Get_should_return_notfound_when_id_is_null()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Edit(id);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Tagastatakse NotFound, kui id on null
        }

        [Fact]
        public async Task Edit_Get_should_return_notfound_when_car_not_found()
        {
            // Arrange
            var carId = 999; // See auto ei ole andmebaasis
            _carServiceMock.Setup(x => x.Get(carId)).ReturnsAsync((Car)null); // Simuleeri, et autot ei leita

            // Act
            var result = await _controller.Edit(carId);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Tagastatakse NotFound, kui autot ei leita
        }

        [Fact]
        public async Task Edit_Get_should_return_view_when_car_exists()
        {
            // Arrange
            var carId = 1;
            var car = new Car { Id = carId, Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true };
            _carServiceMock.Setup(x => x.Get(carId)).ReturnsAsync(car); // Simuleeri, et auto on olemas

            // Act
            var result = await _controller.Edit(carId) as ViewResult;

            // Assert
            Assert.NotNull(result); // Veenduge, et tulemuseks on ViewResult
            var model = Assert.IsType<Car>(result.Model); // Kontrollige, et mudel oleks õige tüüp
            Assert.Equal(carId, model.Id); // Veenduge, et mudeli ID vastab sisendist saadud ID-le
        }

        [Fact]
        public async Task Edit_should_return_notfound_when_id_does_not_match_car_id()
        {
            // Arrange
            var carId = 1; // ID, mis tuleb URL-ist
            var car = new Car { Id = 2, Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true }; // Vale ID

            // Act
            var result = await _controller.Edit(carId, car);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Kui ID-d ei klapi, siis tagastatakse NotFound
        }

        [Fact]
        public async Task Edit_should_return_view_when_model_is_invalid()
        {
            // Arrange
            var carId = 1;
            var car = new Car { Id = carId, Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true };
            _controller.ModelState.AddModelError("Title", "Title is required"); // Lisa mudeli viga, et muudatus oleks kehtetu

            // Act
            var result = await _controller.Edit(carId, car) as ViewResult;

            // Assert
            Assert.NotNull(result); // Veenduge, et tulemuseks oleks ViewResult
            Assert.Equal(car, result.Model); // Tagastatud mudel peaks olema sama, mis me andsime
            Assert.False(_controller.ModelState.IsValid); // ModelState peaks olema kehtetu
        }

        [Fact]
        public async Task Edit_should_redirect_to_index_when_model_is_valid()
        {
            // Arrange
            var carId = 1;
            var car = new Car { Id = carId, Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true };
            _carServiceMock.Setup(x => x.Save(It.IsAny<Car>())).Returns(Task.CompletedTask); // Mockige salvestamine

            // Act
            var result = await _controller.Edit(carId, car) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result); // Veenduge, et tagastatakse RedirectToActionResult
            Assert.Equal("Index", result.ActionName); // Suunamine peaks olema "Index" lehele
            _carServiceMock.Verify(x => x.Save(It.IsAny<Car>()), Times.Once); // Veenduge, et salvestamise meetod on kutsutud
        }


        // Delete Tests

        [Fact]
        public async Task Delete_should_return_notfound_when_id_is_null()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_notfound_when_car_not_found()
        {
            // Arrange
            var carId = 999;
            _carServiceMock.Setup(x => x.Get(carId)).ReturnsAsync((Car)null);

            // Act
            var result = await _controller.Delete(carId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_view_when_car_exists()
        {
            // Arrange
            var carId = 1;
            var car = new Car { Id = carId, Type = "Sedan", RegistrationNumber = "ABC123", HourlyRate = 20, KmRate = 0.2, IsAvaliable = true };
            _carServiceMock.Setup(x => x.Get(carId)).ReturnsAsync(car);

            // Act
            var result = await _controller.Delete(carId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Car>(result.Model);
            Assert.Equal(carId, model.Id);
        }

        [Fact]
        public async Task DeleteConfirmed_should_redirect_to_index()
        {
            // Arrange
            var carId = 1;
            _carServiceMock.Setup(x => x.Delete(carId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(carId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _carServiceMock.Verify(x => x.Delete(carId), Times.Once);
        }
    }
}
