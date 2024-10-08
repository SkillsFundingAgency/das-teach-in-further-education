using Xunit;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Controllers;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Exceptions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Contentful.Core.Models;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Controllers
{
    public class LandingControllerTests
    {
        private readonly IContentModelService _contentModelService;
        private readonly LandingController _controller;

        public LandingControllerTests()
        {
            _contentModelService = A.Fake<IContentModelService>();
            _controller = new LandingController(_contentModelService);
        }

        [Fact]
        public void Landing_WhenPageModelIsNull_ThrowsPageNotFoundException()
        {
            // Arrange
            var pageUrl = "home";
            A.CallTo(() => _contentModelService.GetPageContentModel(pageUrl)).Returns(null as PageContentModel);

            // Act & Assert
            var exception = Assert.Throws<PageNotFoundException>(() => _controller.Landing(pageUrl));

            Assert.Equal($"The requested url {pageUrl} could not be found", exception.Message);
        }

        [Fact]
        public void Landing_WhenPageModelIsNotNull_ReturnsLandingView()
        {
            // Arrange
            var pageUrl = "home";
            var pageModel = new PageContentModel
            {
                PageURL = pageUrl,
                PageTitle = "Home Page",
                PageTemplate = "DefaultTemplate",
                Breadcrumbs = null, // Optional
                PageComponents = new List<IContent>() // Optional
            };
            A.CallTo(() => _contentModelService.GetPageContentModel(pageUrl)).Returns(pageModel);

            // Act
            var result = _controller.Landing(pageUrl) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Landing", result.ViewName);
            Assert.Equal(pageModel, result.Model);
        }

        [Fact]
        public async Task PagePreview_WhenPageModelIsNull_ThrowsPageNotFoundException()
        {
            // Arrange
            var pageUrl = "home";
            A.CallTo(() => _contentModelService.GetPagePreviewModel(pageUrl)).Returns(Task.FromResult<PageContentModel>(null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PageNotFoundException>(() => _controller.PagePreview(pageUrl));

            Assert.Equal($"The requested url {pageUrl} could not be found", exception.Message);
        }

        [Fact]
        public async Task PagePreview_WhenPageModelIsNotNull_ReturnsLandingView()
        {
            // Arrange
            var pageUrl = "home";
            var pageModel = new PageContentModel
            {
                PageURL = pageUrl,
                PageTitle = "Home Page",
                PageTemplate = "DefaultTemplate",
                Breadcrumbs = null, // Optional
                PageComponents = new List<IContent>() // Optional
            };
            A.CallTo(() => _contentModelService.GetPagePreviewModel(pageUrl)).Returns(Task.FromResult(pageModel));

            // Act
            var result = await _controller.PagePreview(pageUrl) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Landing", result.ViewName);
            Assert.Equal(pageModel, result.Model);
        }
    }
}
