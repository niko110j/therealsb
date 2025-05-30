using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;
using SB2.Models.ViewModels;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace SB2.Controllers
{
    public class AllOrdersPageController : RenderController
    {
        private readonly IUmbracoDatabase _db;

        public AllOrdersPageController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _db = databaseFactory.CreateDatabase();
        }
        [HttpGet]
        public IActionResult Index(ContentModel model)
        {

            var orders = _db.Fetch<Order>("SELECT * FROM Orders ORDER BY Created DESC");

            var viewModel = new AllOrdersViewModel(model.Content)
            {

                Orders = orders.Select(o => new OrderListItem
                {
                    //Name = o.Id,
                    ClientName = o.ClientName,
                    ClientEmail = o.ClientEmail,
                    SalespersonName = o.SalespersonName,
                    FilledBy = o.FilledBy,
                    Status = o.Status,
                    BookingType = o.BookingType,
                    Created = o.Created
                }).ToList()
            };
            return View("allOrdersPage", viewModel);
        }
    }
}
