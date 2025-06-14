﻿using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SB2.Controllers;
using SB2.Models.ViewModels;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Models;
using System.Collections.Generic;

namespace SB2.Tests;

[TestFixture]
public class OrderControllerTests
{
    private OrderController _controller;
    private Mock<IUmbracoDatabase> _mockDb;

    [SetUp]
    public void SetUp()
    {
        // Create mocks for the services your controller needs
        var mockContextAccessor = new Mock<IUmbracoContextAccessor>();
        var mockDbFactory = new Mock<IUmbracoDatabaseFactory>();
        _mockDb = new Mock<IUmbracoDatabase>();
        ServiceContext mockServices = null!;

        var mockCaches = new Mock<AppCaches>();
        var mockLogger = new Mock<IProfilingLogger>();
        var mockUrlProvider = new Mock<IPublishedUrlProvider>();
        var mockContentService = new Mock<IContentService>();
        var mockMemberService = new Mock<IMemberService>();

        mockDbFactory.Setup(f => f.CreateDatabase()).Returns(_mockDb.Object);

        _controller = new OrderController(
            mockContextAccessor.Object,
            mockDbFactory.Object,
            mockServices,
            mockCaches.Object,
            mockLogger.Object,
            mockUrlProvider.Object,
            mockContentService.Object,
            mockMemberService.Object
        );
    }

    [Test]
    public void CreateOrder_WithValidModel_ReturnsRedirectResult()
    {
        // Arrange
        var model = new OrderFormViewModel
        {
            ClientName = "Test Company",
            ClientEmail = "test@example.com",
            SalespersonName = "Salesperson",
            FilledBy = "Someone",
            BookingType = "Print",
            BookingFields = new Dictionary<string, string> {
                { "field1", "value1" }
            }
        };

        // Act
        var result = _controller.CreateOrder(model);

        // Assert
        Assert.That(result, Is.InstanceOf<RedirectResult>());
        var redirect = result as RedirectResult;
        Assert.That(redirect!.Url, Is.EqualTo("/allorderpage"));

    }

    [Test]
    public void CreateOrder_WithInvalidModel_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("ClientName", "Required");
        var model = new OrderFormViewModel();

        // Act
        var result = _controller.CreateOrder(model);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}
