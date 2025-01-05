using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using UrlShortenerContainerized.Controllers;
using UrlShortenerContainerized.Models;
using UrlShortenerContainerized.Repositories;

namespace UrlShortenerContainerized.Tests;

public class HomeControllerTests
{
    [Test]
    public void AddUrl_Works()
    {
        //Arrange
        var logger = A.Fake<ILogger<HomeController>>();
        var repo = A.Fake<IUrlRepository>();

        var httpContextMock = A.Fake<HttpContext>();
        var requestMock = A.Fake<HttpRequest>();
        var routeData = new RouteData
        {
            Values =
            {
                ["controller"] = "Home",
                ["action"] = "AddUrl"
            }
        };
        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = "Home",
            ActionName = "AddUrl"
        };
        
        // Setup mock request
        A.CallTo(() => requestMock.Scheme).Returns("http");
        A.CallTo(() => requestMock.Host).Returns(new HostString("localhost"));
        A.CallTo(() => requestMock.Query).Returns(new QueryCollection());
        A.CallTo(() => httpContextMock.Request).Returns(requestMock);
        
        var actionContext = new ActionContext(httpContextMock, routeData, actionDescriptor);
        
        var tempDataProvider = A.Fake<ITempDataProvider>();
        var tempData = new TempDataDictionary(httpContextMock, tempDataProvider);
        
        var controller = new HomeController(logger, repo){ TempData = tempData, ControllerContext = new ControllerContext(actionContext) };

        //Act
        var result = controller.AddUrl(new AddUrlModel() { FullUrl = "https://www.redhat.com/en" });

        //Assert
        A.CallTo(() => repo.Add(A<string>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<ViewResult>());
        Assert.That((result as ViewResult)!.Model, Is.Not.Null);

        var model = (result as ViewResult)!.Model as LinkResultModel;
        Assert.That(model!.Url, Does.StartWith("http://localhost"));
    }

    [Test]
    public void RedirectTo_Works()
    {
        //Arrange
        var logger = A.Fake<ILogger<HomeController>>();
        const string key = "00000000000000000000000000000001";
        const string url = "https://www.redhat.com/en";
        
        var repo = A.Fake<IUrlRepository>();
        A.CallTo(() => repo.Get(key)).Returns(url);

        var controller = new HomeController(logger, repo);
        
        //Act
        var result = controller.RedirectTo(key);
        
        //Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.TypeOf<RedirectResult>());

        var redirectResult = result as RedirectResult;
        Assert.That(redirectResult!.Url, Is.EqualTo(url));
    }
}