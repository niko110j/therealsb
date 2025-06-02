using Microsoft.AspNetCore.Mvc;
using SB2.Models.ViewModels;
using System.Text.Json;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models;
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
        private readonly IMemberService _memberService;

        public OrderController(
        IUmbracoContextAccessor contextAccessor,
        IUmbracoDatabaseFactory dbFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger logger,
        IPublishedUrlProvider urlProvider,
        IContentService contentService,
        IMemberService memberService)
        : base(contextAccessor, dbFactory, services, appCaches, logger, urlProvider)
        {
            _contentService = contentService;
            _db = dbFactory.CreateDatabase();
            _memberService = memberService;
            _memberService = memberService;
        }
    

        [HttpPost]

        public IActionResult CreateOrder(OrderFormViewModel model)
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
                Status = model.Status,
                BookingType = model.BookingType,
                BookingFields = bookingFieldsJson,
                Created = DateTime.UtcNow
            };


            // Save to the DB
            _db.Insert(order);

            foreach (var field in model.BookingFields)
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                var bookingField = new BookingField
                {
                    OrderId = order.Id,
                    FieldKey = field.Key,
                    FieldValue = field.Value
                };
#pragma warning restore CS8601 // Possible null reference assignment.

                _db.Insert(bookingField);
            }

            //TempData["SuccessMessage"] = "Ordre og bookingdetaljer gemt!";
            return Redirect("/allorderpage");


        }


    }

    
}
