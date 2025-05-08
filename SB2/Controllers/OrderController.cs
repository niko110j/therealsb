using Microsoft.AspNetCore.Mvc;
using SB2.Models.ViewModels;
using System.Text.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace SB2.Controllers { 
public class OrderController : SurfaceController
{
    private readonly IContentService _contentService;
        private readonly IUmbracoDatabase _db;

        public OrderController(
        IUmbracoContextAccessor contextAccessor,
        IUmbracoDatabaseFactory dbFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger logger,
        IPublishedUrlProvider urlProvider,
        IContentService contentService)
        : base(contextAccessor, dbFactory, services, appCaches, logger, urlProvider)
    {
        _contentService = contentService;
            _db = dbFactory.CreateDatabase();
        }

    [HttpGet] //Is this needed? 
    public IActionResult LoadBookingPartial(string type)
    {
        if (string.IsNullOrEmpty(type)) return Content(""); // No selection made

        switch (type.ToLower())
        {
            case "print":
                return PartialView("~/Views/Partials/_PrintOrder.cshtml");
            case "radio":
                return PartialView("~/Views/Partials/_RadioOrder.cshtml");
            case "digital":
                return PartialView("~/Views/Partials/_DigitalOrder.cshtml");
            default:
                return Content(""); // fallback
        }
    }

        [HttpPost]

        public IActionResult CreateOrder(Order model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Model state invalid: " + string.Join(", ", errors));
            }

            // Extract dynamic booking fields as JSON
            var bookingFieldsJson = JsonSerializer.Serialize(model.BookingFields);

            var order = new Order
            {
                ClientName = model.ClientName,
                ClientEmail = model.ClientEmail,
                SalespersonName = model.SalespersonName,
                FilledBy = model.FilledBy,
                BookingType = model.BookingType,
                BookingFields = bookingFieldsJson,
                Created = DateTime.UtcNow
            };

            // Save to the DB
            _db.Insert(order); // ← assumes _db is your NPoco database instance

            TempData["SuccessMessage"] = "Ordre gemt!";
            return Redirect("/allorderpage");
        }



    }
}
