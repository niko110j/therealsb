using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SB2.Models.ViewModels;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Controllers;
using System.Text.Json;

namespace SB2.Controllers
{
    public class OverviewController : RenderController
    {
        private readonly IUmbracoDatabase _db;
        private readonly IMemberManager _memberManager;

        public OverviewController(
            ILogger<RenderController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory, IMemberManager memberManager)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _db = databaseFactory.CreateDatabase();
            _memberManager = memberManager;
        }
     
        [HttpGet]
        public async Task<IActionResult> Index(ContentModel model)
        {
            var currentMember = await _memberManager.GetCurrentMemberAsync();

            if (currentMember == null)
            {
                return Unauthorized(); 
            }

            var allOrders = _db.Fetch<Order>("SELECT * FROM Orders ORDER BY Created DESC");

            var filteredOrders = allOrders
                .Where(order =>
                    order != null &&
                    !string.Equals(order.Status, "kladde", StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(order.ClientName?.Trim(), currentMember.Name?.Trim(), StringComparison.OrdinalIgnoreCase)
                )
                .ToList();

            var viewModel = new AllOrdersViewModel(model.Content)
            {
                Orders = filteredOrders.Select(o => new OrderListItem
                {
                    Id = o.Id,
                    ClientName = o.ClientName,
                    ClientEmail = o.ClientEmail,
                    SalespersonName = o.SalespersonName,
                    FilledBy = o.FilledBy,
                    Status = o.Status,
                    BookingType = o.BookingType,
                    BookingFieldsJson = o.BookingFields,
                    Created = o.Created
                }).ToList()
            };

            return View("overview", viewModel);
        }




    }
}
