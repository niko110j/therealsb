using Microsoft.AspNetCore.Mvc;
using SB2.Models.ViewModels;
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
    public IActionResult SubmitOrder(OrderFormViewModel model)
    {
        if (!ModelState.IsValid)
            return CurrentUmbracoPage();

        //if (model.SelectedBookingType == "Print")
        //{
        //    if (model.PrintQuantity == null || model.PrintUnitPrice == null)
        //        ModelState.AddModelError("", "Udfyld alle felter for Print-orderen.");
        //}


        // Combine general and booking-specific info here and save
        // Redirect to confirmation page or orders list
        return RedirectToCurrentUmbracoPage();
    }

}
}
