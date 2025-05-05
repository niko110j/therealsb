using Microsoft.AspNetCore.Mvc;
using SB2.Models.ViewModels;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Website.Controllers;

namespace SB2.Controllers
{
    public class OrdersOverviewController : SurfaceController
    {
        
        private readonly IContentService _contentService;

        public OrdersOverviewController(IUmbracoContextAccessor contextAccessor,
            IUmbracoDatabaseFactory dbFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger logger,
            IPublishedUrlProvider urlProvider,
            IContentService contentService)
            : base(contextAccessor, dbFactory, services, appCaches, logger, urlProvider)
        {
            _contentService = contentService;
        }

      
        [HttpGet]
        public IActionResult Index()
        {
            var model = new AllOrdersViewModel();

            // Your orders container ID
            var container = _contentService.GetById(Guid.Parse("0f415ccc-12c3-4ab2-a58d-a2e8f568d41c"));
            if (container != null)
            {
                var children = _contentService.GetPagedChildren(container.Id, 0, 100, out var _);
                foreach (var child in children)
                {
                    model.Orders.Add(new OrderListItem
                    {
                        Name = child.Name,
                        ClientName = child.GetValue<string>("clientName"),
                        ClientEmail = child.GetValue<string>("clientEmail"),
                        BookingType = child.GetValue<string>("bookingType")
                    });
                }
            }

            return View("allOrders", model); // View: Views/OrdersOverview/AllOrders.cshtml
        }
    }

}

