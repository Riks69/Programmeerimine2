using KooliProjekt.Controllers;
using KooliProjekt.Data;
using KooliProjekt.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [Fact]
        public async Task Index_should_return_index_viewAsync()
        {
            // Arrange
            var bookings = new PagedResult<Booking>() 
            { 
                Results = {
                    new Booking { UserId = 1, StartTime = new DateTime(2024, 11, 25, 9, 0, 0), EndTime = new DateTime(2024, 11, 28, 9, 0, 0), DistanceKm = 200, IsCompleted = true },
                    new Booking { UserId = 2, StartTime = new DateTime(2024, 2, 6, 9, 0, 0), EndTime = new DateTime(2024, 2, 10, 9, 0, 0), DistanceKm = 195, IsCompleted = true },
                }
            };
            // Act
            _bookingServiceMock.Setup(x => x.List(It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(bookings);
            var result = await _controller.Index() as ViewResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookings, result.Model);

        }

    }
}
