using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.Persistence;
using SB2.Controllers;
using SB2.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.Controllers;

namespace SB2.Tests
{
    [TestClass]
    public class AllOrdersPageControllerTests
    {
        private AllOrdersPageController _controller;
        private Mock<IUmbracoDatabase> _mockDb;

        [TestInitialize]
        public void SetUp()
        {
            var mockLogger = new Mock<ILogger<RenderController>>();
            var mockViewEngine = new Mock<ICompositeViewEngine>();
            var mockContextAccessor = new Mock<IUmbracoContextAccessor>();
            var mockDbFactory = new Mock<IUmbracoDatabaseFactory>();

            _mockDb = new Mock<IUmbracoDatabase>();
            mockDbFactory.Setup(f => f.CreateDatabase()).Returns(_mockDb.Object);

            _controller = new AllOrdersPageController(
                mockLogger.Object,
                mockViewEngine.Object,
                mockContextAccessor.Object,
                mockDbFactory.Object
            );
        }

        [TestMethod]
        public void Index_ReturnsViewResult_WithCorrectViewNameAndModel()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    ClientName = "Client A",
                    ClientEmail = "clienta@example.com",
                    SalespersonName = "Salesperson A",
                    FilledBy = "User1",
                    Status = "Kladde",
                    BookingType = "Print",
                    Created = System.DateTime.UtcNow
                }
            };

            _mockDb.Setup(db => db.Fetch<Order>("SELECT * FROM Orders ORDER BY Created DESC")).Returns(orders);

            var mockContent = new Mock<IPublishedContent>();
            var model = new ContentModel(mockContent.Object);

            // Act
            var result = _controller.Index(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("allOrdersPage", result.ViewName);

            var viewModel = result.Model as AllOrdersViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(1, viewModel.Orders.Count);
            Assert.AreEqual("Client A", viewModel.Orders.First().ClientName);
        }

        [TestMethod]
        public void Index_WhenNoOrdersExist_ReturnsEmptyModel()
        {
            // Arrange
            _mockDb.Setup(db => db.Fetch<Order>("SELECT * FROM Orders ORDER BY Created DESC")).Returns(new List<Order>());

            var mockContent = new Mock<IPublishedContent>();
            var model = new ContentModel(mockContent.Object);

            // Act
            var result = _controller.Index(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var viewModel = result.Model as AllOrdersViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(0, viewModel.Orders.Count);
        }
    }
}
