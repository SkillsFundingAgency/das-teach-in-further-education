using Contentful.Core.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.TeachInFurtherEducation.Web.Controllers;
using SFA.DAS.TeachInFurtherEducation.Web.Exceptions;
using SFA.DAS.TeachInFurtherEducation.Web.Infrastructure;
using SFA.DAS.TeachInFurtherEducation.Web.Models;
using SFA.DAS.TeachInFurtherEducation.Web.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace SFA.DAS.TeachInFurtherEducation.UnitTests.Web.Controllers;

public class LandingControllerTests
{
    private readonly IContentModelService _contentModelService;
    private readonly LandingController _controller;
    private const string RouteName = RouteNames.Home;

    public LandingControllerTests()
    {
        _contentModelService = A.Fake<IContentModelService>();
        _controller = new LandingController(_contentModelService);
        var httpContext = new DefaultHttpContext
        {
            Session = A.Fake<ISession>()
        };
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void Landing_WhenPageModelIsNull_ThrowsPageNotFoundException()
    {
        // Arrange

        A.CallTo(() => _contentModelService.GetPageContentModel(RouteName, false)).Returns(null as PageContentModel);

        // Act & Assert
        var exception = Assert.Throws<PageNotFoundException>(() => _controller.Landing(RouteName));

        Assert.Equal($"The requested url {RouteName} could not be found", exception.Message);
    }

    [Fact]
    public void Landing_WhenPageModelIsNotNull_ReturnsLandingView()
    {
        // Arrange

        var pageModel = new PageContentModel
        {
            PageURL = RouteName,
            PageTitle = "Home Page",
            PageTemplate = "DefaultTemplate",
            Breadcrumbs = null, // Optional
            PageComponents = new List<IContent>() // Optional
        };
        
           

            
        A.CallTo(() => _contentModelService.GetPageContentModel(RouteName, false)).Returns(pageModel);
         
        // Act
        var result = _controller.Landing(RouteName) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Landing", result.ViewName);
        Assert.Equal(pageModel, result.Model);
    }

    [Fact]
    public async Task PagePreview_WhenPageModelIsNull_ThrowsPageNotFoundException()
    {
        // Arrange
        A.CallTo(() => _contentModelService.GetPagePreviewModel(RouteName,false))
            .Returns(Task.FromResult<PageContentModel>(null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PageNotFoundException>(() => _controller.PagePreview(RouteName));

        Assert.Equal($"The requested url {RouteName} could not be found", exception.Message);
    }

    [Fact]
    public async Task PagePreview_WhenPageModelIsNotNull_ReturnsLandingView()
    {
        // Arrange
        var pageModel = new PageContentModel
        {
            PageURL = RouteName,
            PageTitle = "Home Page",
            PageTemplate = "DefaultTemplate",
            Breadcrumbs = null, // Optional
            PageComponents = new List<IContent>() // Optional
        };
        A.CallTo(() => _contentModelService.GetPagePreviewModel(RouteName,false)).Returns(Task.FromResult(pageModel));

        // Act
        var result = await _controller.PagePreview(RouteName) as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Landing", result.ViewName);
        Assert.Equal(pageModel, result.Model);
    }
}