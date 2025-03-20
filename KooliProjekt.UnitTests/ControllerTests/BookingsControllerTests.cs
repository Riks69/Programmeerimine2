using KooliProjekt.Controllers;
using KooliProjekt.Data;
using KooliProjekt.Services;
using Microsoft.AspNetCore.Mvc;
using KooliProjekt.Models;
using KooliProjekt.Search;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KooliProjekt.UnitTests.ControllerTests
{
    public class BookingsControllerTests
    {
        private readonly BookingsController _controller;
        private readonly Mock<IBookingService> _bookingServiceMock;

        public BookingsControllerTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _controller = new BookingsController(_bookingServiceMock.Object);
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
        public async Task Details_should_return_not_found_when_booking_does_not_exist()
        {
            var bookingId = 999;
            _bookingServiceMock.Setup(x => x.Get(bookingId)).ReturnsAsync((Booking)null);

            var result = await _controller.Details(bookingId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_should_return_view_when_booking_exists()
        {
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, UserId = 1, CarId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), DistanceKm = 100 };
            _bookingServiceMock.Setup(x => x.Get(bookingId)).ReturnsAsync(booking);

            var result = await _controller.Details(bookingId) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<Booking>(result.Model);
            Assert.Equal(bookingId, model.Id);
        }

        // Create Tests

        [Fact]
        public void Create_should_return_view()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.ViewName); // Veenduge, et ei ole määratud eraldi vaate nime
        }

        [Fact]
        public async Task Create_should_return_view_when_model_is_invalid()
        {
            var booking = new Booking { UserId = 0, CarId = 0, StartTime = DateTime.Now, EndTime = DateTime.Now, DistanceKm = 0 };
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            var result = await _controller.Create(booking) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(booking, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_should_redirect_to_index_when_model_is_valid()
        {
            var booking = new Booking { UserId = 1, CarId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), DistanceKm = 100 };
            _bookingServiceMock.Setup(x => x.Save(It.IsAny<Booking>())).Returns(Task.CompletedTask);

            var result = await _controller.Create(booking) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _bookingServiceMock.Verify(x => x.Save(It.IsAny<Booking>()), Times.Once);
        }

        // Index Tests

        [Fact]
        public async Task Index_should_return_index_viewAsync()
        {
            var bookings = new PagedResult<Booking>
            {
                Results = new List<Booking> {
                    new Booking { UserId = 1, StartTime = new DateTime(2024, 11, 25, 9, 0, 0), EndTime = new DateTime(2024, 11, 28, 9, 0, 0), DistanceKm = 200, IsCompleted = true },
                    new Booking { UserId = 2, StartTime = new DateTime(2024, 2, 6, 9, 0, 0), EndTime = new DateTime(2024, 2, 10, 9, 0, 0), DistanceKm = 195, IsCompleted = true }
                }
            };

            _bookingServiceMock.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(bookings);

            var result = await _controller.Index() as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<BookingIndexModel>(result.Model);
            Assert.Equal(bookings.Results.Count, model.Data.Results.Count);
        }

        // Edit Tests (GET)

        [Fact]
        public async Task Edit_should_return_not_found_when_id_is_null()
        {
            int? id = null;
            var result = await _controller.Edit(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_should_return_not_found_when_booking_does_not_exist()
        {
            var bookingId = 999; // Oletame, et see ID ei eksisteeri
            _bookingServiceMock.Setup(x => x.Get(bookingId)).ReturnsAsync((Booking)null);

            var result = await _controller.Edit(bookingId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_should_return_view_when_booking_exists()
        {
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, UserId = 1, CarId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), DistanceKm = 100 };
            _bookingServiceMock.Setup(x => x.Get(bookingId)).ReturnsAsync(booking);

            var result = await _controller.Edit(bookingId) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<Booking>(result.Model);
            Assert.Equal(bookingId, model.Id);
        }

        // Edit Tests (POST)

        [Fact]
        public async Task Edit_should_return_not_found_when_id_mismatch()
        {
            var bookingId = 1;
            var booking = new Booking { Id = 2, UserId = 1, CarId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), DistanceKm = 100 };

            var result = await _controller.Edit(bookingId, booking);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_should_redirect_to_index_when_model_is_valid()
        {
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, UserId = 1, CarId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), DistanceKm = 100 };
            _bookingServiceMock.Setup(x => x.Save(It.IsAny<Booking>())).Returns(Task.CompletedTask);

            var result = await _controller.Edit(bookingId, booking) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _bookingServiceMock.Verify(x => x.Save(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task Edit_should_return_view_when_model_is_invalid()
        {
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, UserId = 1, CarId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), DistanceKm = 100 };
            _controller.ModelState.AddModelError("UserId", "UserId is required");

            var result = await _controller.Edit(bookingId, booking) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(booking, result.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        // Delete Tests

        [Fact]
        public async Task DeleteConfirmed_should_redirect_to_index()
        {
            var bookingId = 1;
            _bookingServiceMock.Setup(x => x.Delete(bookingId)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(bookingId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            _bookingServiceMock.Verify(x => x.Delete(bookingId), Times.Once);
        }

        [Fact]
        public async Task Delete_should_return_not_found_when_id_is_null()
        {
            int? id = null;

            var result = await _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_not_found_when_booking_does_not_exist()
        {
            var bookingId = 999;
            _bookingServiceMock.Setup(x => x.Get(bookingId)).ReturnsAsync((Booking)null);

            var result = await _controller.Delete(bookingId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_should_return_view_when_booking_exists()
        {
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, UserId = 1, CarId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddDays(1), DistanceKm = 100 };
            _bookingServiceMock.Setup(x => x.Get(bookingId)).ReturnsAsync(booking);

            var result = await _controller.Delete(bookingId) as ViewResult;

            Assert.NotNull(result);
            var model = Assert.IsType<Booking>(result.Model);
            Assert.Equal(bookingId, model.Id);
        }
    }
}
