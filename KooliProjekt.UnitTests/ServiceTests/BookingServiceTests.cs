using KooliProjekt.Data;
using KooliProjekt.Data.Repositories;
using KooliProjekt.Services;
using Moq;
using Xunit;

namespace KooliProjekt.UnitTests.ServiceTests
{
    public class BookingServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IBookingRepository> _repositoryMock;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _repositoryMock = new Mock<IBookingRepository>();
            _bookingService = new BookingService(_uowMock.Object);

            _uowMock.SetupGet(r => r.BookingRepository)
                    .Returns(_repositoryMock.Object);
        }

        [Fact]
        public async Task List_should_return_list_of_bookings()
        {
            // Arrange
            var results = new List<Booking>
            {
                new Booking { Id = 1 },
                new Booking { Id = 2 }
            };
            var pagedResult = new PagedResult<Booking> { Results = results };
            _repositoryMock.Setup(r => r.List(It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync(pagedResult);

            // Act
            var result = await _bookingService.List(1, 10);

            // Assert
            Assert.Equal(pagedResult, result);
        }
    }
}