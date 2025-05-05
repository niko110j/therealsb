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
    }

    [HttpGet]
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
       
        public IActionResult CreateOrder(OrderFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest("Model state invalid: " + string.Join(", ", errors));
            }


            // Get the parent node (Orders container)
            var ordersContainer = _contentService.GetById(Guid.Parse("0f415ccc-12c3-4ab2-a58d-a2e8f568d41c"));
            if (ordersContainer == null)
                return StatusCode(500, "Order container not found");

            // Create order node
            var order = _contentService.Create($"Order - {DateTime.Now:yyyyMMddHHmmss}", ordersContainer.Id, "order");

            order.SetValue("clientName", model.ClientName);
            order.SetValue("clientEmail", model.ClientEmail);
            order.SetValue("clientPhone", model.ClientPhone);
            order.SetValue("salespersonName", model.SalespersonName);
            order.SetValue("filledBy", model.FilledBy);
            order.SetValue("bookingType", model.BookingType);
            order.SetValue("bookingFields", JsonSerializer.Serialize(model.BookingFields));

            _contentService.Save(order);

            TempData["SuccessMessage"] = "Ordre gemt!";
            return Redirect("/allorderpage");
        }


    }
}
