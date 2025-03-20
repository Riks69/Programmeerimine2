using KooliProjekt.Controllers;
using KooliProjekt.Models;
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
            Assert.True(result.ViewName == "Index" ||
                        string.IsNullOrEmpty(result.ViewName));
        }

        // Privacy Test
        [Fact]
        public void Privacy_should_return_view()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Privacy() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result.ViewName)); // If view name is null or empty, it's the default view.
        }

        // Error Test
        [Fact]
        public void Error_should_return_view_with_error_model_and_request_id_and_cache_settings()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Error() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<ErrorViewModel>(result.Model); // Ensure the model is of type ErrorViewModel
            Assert.NotNull(model.RequestId); // Ensure RequestId is not null

            // Verify that RequestId is assigned correctly
            Assert.NotEmpty(model.RequestId); // Ensure that RequestId is not empty (since it's used in the view)

            // Optional: Verify that ResponseCache is applied correctly
            var cacheAttribute = (ResponseCacheAttribute)Attribute.GetCustomAttribute(
                controller.GetType().GetMethod("Error"), typeof(ResponseCacheAttribute));

            Assert.NotNull(cacheAttribute); // Ensure ResponseCache attribute is present
            Assert.Equal(0, cacheAttribute.Duration); // Duration should be 0
            Assert.Equal(ResponseCacheLocation.None, cacheAttribute.Location); // Location should be None
            Assert.True(cacheAttribute.NoStore); // NoStore should be true
        }

        // ShowRequestId Test
        [Fact]
        public void ShowRequestId_should_return_true_if_request_id_is_not_null_or_empty()
        {
            // Arrange
            var model = new ErrorViewModel { RequestId = "some-request-id" };

            // Act
            var result = model.ShowRequestId;

            // Assert
            Assert.True(result); // Should return true because RequestId is not null or empty
        }

        [Fact]
        public void ShowRequestId_should_return_false_if_request_id_is_null_or_empty()
        {
            // Arrange
            var modelWithNullRequestId = new ErrorViewModel { RequestId = null };
            var modelWithEmptyRequestId = new ErrorViewModel { RequestId = "" };

            // Act
            var resultNull = modelWithNullRequestId.ShowRequestId;
            var resultEmpty = modelWithEmptyRequestId.ShowRequestId;

            // Assert
            Assert.False(resultNull); // Should return false because RequestId is null
            Assert.False(resultEmpty); // Should return false because RequestId is empty
        }
    }
}
