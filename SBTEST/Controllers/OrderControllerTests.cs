using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace SBTEST.Controllers
{
    [TestClass]
    public class OrderControllerTests
    {
        private OrderController _controller;
        private Mock<IUmbracoDatabase> _mockDb;

        [TestInitialize]
        public void SetUp()
        {
            // Create mocks for the services your controller needs
            var mockContextAccessor = new Mock<IUmbracoContextAccessor>();
            var mockDbFactory = new Mock<IUmbracoDatabaseFactory>();
            _mockDb = new Mock<IUmbracoDatabase>();
            ServiceContext mockServices = null!; // adjust if you plan to test anything requiring services

            var realAppCaches = AppCaches.NoCache;

            var mockLogger = new Mock<IProfilingLogger>();
            var mockUrlProvider = new Mock<IPublishedUrlProvider>();


            mockDbFactory.Setup(f => f.CreateDatabase()).Returns(_mockDb.Object);

            _controller = new OrderController(
                mockContextAccessor.Object,
                mockDbFactory.Object,
                mockServices,
                realAppCaches,
                mockLogger.Object,
                mockUrlProvider.Object
            );
        }

        [TestMethod]
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
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirect = result as RedirectResult;
            Assert.AreEqual("/allorderpage", redirect!.Url);
        }

        [TestMethod]
        public void CreateOrder_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("ClientName", "Required");
            var model = new OrderFormViewModel();

            // Act
            var result = _controller.CreateOrder(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void DuplicateOrder_WithValidId_InsertsAndRedirects()
        {
            // Arrange
            var originalOrder = new Order
            {
                Id = 42,
                ClientName = "Original",
                ClientEmail = "original@test.com",
                SalespersonName = "Sales",
                FilledBy = "Someone",
                Status = "Godkendt",
                BookingType = "Radio",
                BookingFields = "{\"field1\":\"value1\"}",
                Created = DateTime.UtcNow
            };

            _mockDb.Setup(db => db.SingleOrDefault<Order>("WHERE Id = @0", 42))
                   .Returns(originalOrder);

            // Act
            var result = _controller.DuplicateOrder(42);

            // Assert
            _mockDb.Verify(db => db.Insert(It.Is<Order>(o =>
                o.ClientName == originalOrder.ClientName &&
                o.Status == "Kladde" // duplicated status
            )), Times.Once);

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirect = result as RedirectResult;
            Assert.AreEqual("/allorderpage", redirect!.Url);
        }

        [TestMethod]
        public void ChangeStatus_WithValidId_UpdatesStatusAndRedirects()
        {
            // Arrange
            var existingOrder = new Order
            {
                Id = 55,
                Status = "Kladde"
            };

            _mockDb.Setup(db => db.SingleOrDefault<Order>("WHERE Id = @0", 55))
                   .Returns(existingOrder);

            // Act
            var result = _controller.ChangeStatus(55, "Godkendt");

            // Assert
            _mockDb.Verify(db => db.Update(It.Is<Order>(o =>
                o.Id == 55 && o.Status == "Godkendt"
            )), Times.Once);

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirect = result as RedirectResult;
            Assert.AreEqual("/allorderpage", redirect!.Url);
        }

        [TestMethod]
        public void DuplicateOrder_WithInvalidId_DoesNotInsert_Redirects()
        {
            // Arrange: simulate no order found
            _mockDb.Setup(db => db.SingleOrDefault<Order>("WHERE Id = @0", 999))
                   .Returns((Order)null!);

            // Act
            var result = _controller.DuplicateOrder(999);

            // Assert
            _mockDb.Verify(db => db.Insert(It.IsAny<Order>()), Times.Never);

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirect = result as RedirectResult;
            Assert.AreEqual("/allorderpage", redirect!.Url);
        }

        [TestMethod]
        public void ChangeStatus_WithInvalidId_DoesNotUpdate_Redirects()
        {
            // Arrange: simulate no order found
            _mockDb.Setup(db => db.SingleOrDefault<Order>("WHERE Id = @0", 888))
                   .Returns((Order)null!);

            // Act
            var result = _controller.ChangeStatus(888, "Afvist");

            // Assert
            _mockDb.Verify(db => db.Update(It.IsAny<Order>()), Times.Never);

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirect = result as RedirectResult;
            Assert.AreEqual("/allorderpage", redirect!.Url);
        }

    }
}

