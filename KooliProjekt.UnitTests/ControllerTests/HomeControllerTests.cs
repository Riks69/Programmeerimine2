using KooliProjekt.Controllers;
using KooliProjekt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace KooliProjekt.UnitTests.ControllerTests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_should_return_index_view()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ViewName == "Index" || string.IsNullOrEmpty(result.ViewName));
        }

        [Fact]
        public void Privacy_should_return_index_view()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Privacy() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ViewName == "Privacy" || string.IsNullOrEmpty(result.ViewName));
        }

        [Fact]
        public void Error_should_return_error_view_with_error_model()
        {
            // Arrange
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = controller.Error() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ViewName == "Error" || string.IsNullOrEmpty(result.ViewName));

            // Verify that the model is of type ErrorViewModel
            var model = Assert.IsType<ErrorViewModel>(result.Model);

            // Ensure RequestId is set correctly
            Assert.NotNull(model.RequestId);
            Assert.False(string.IsNullOrEmpty(model.RequestId)); // Ensure that RequestId is not null or empty

            // Ensure ShowRequestId works correctly
            Assert.True(model.ShowRequestId); // If RequestId is not null, ShowRequestId should return true
        }
    }
}
