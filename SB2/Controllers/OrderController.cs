using Microsoft.AspNetCore.Mvc;
using SB2.Models.ViewModels;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

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
        return type switch
        {
            "Print" => PartialView("BookingTypes/_PrintOrder"),
            "Radio" => PartialView("BookingTypes/_RadioOrder"),
            "Digital" => PartialView("BookingTypes/_DigitalOrder"),
            _ => Content("")
        };
    }

    [HttpPost]
    public IActionResult SubmitOrder(OrderFormViewModel model)
    {
        if (!ModelState.IsValid)
            return CurrentUmbracoPage();

        if (model.SelectedBookingType == "Print")
        {
            if (model.PrintQuantity == null || model.PrintUnitPrice == null)
                ModelState.AddModelError("", "Udfyld alle felter for Print-orderen.");
        }


        // Combine general and booking-specific info here and save
        // Redirect to confirmation page or orders list
        return RedirectToCurrentUmbracoPage();
    }

}

