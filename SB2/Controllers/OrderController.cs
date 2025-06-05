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
       
        private readonly IUmbracoDatabase _db;
       

        public OrderController(
        IUmbracoContextAccessor contextAccessor,
        IUmbracoDatabaseFactory dbFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger logger,
        IPublishedUrlProvider urlProvider
        
       )
        : base(contextAccessor, dbFactory, services, appCaches, logger, urlProvider)
        {
            
            _db = dbFactory.CreateDatabase();
           
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

        [HttpPost]
        public IActionResult DuplicateOrder(int orderId)
        {
            // Fetch the original order
            var existingOrder = _db.SingleOrDefault<Order>("WHERE Id = @0", orderId);

            if (existingOrder != null)
            {
                var duplicatedOrder = new Order
                {
                    ClientName = existingOrder.ClientName,
                    ClientEmail = existingOrder.ClientEmail,
                    SalespersonName = existingOrder.SalespersonName,
                    FilledBy = existingOrder.FilledBy,
                    Status = "Kladde", 
                    BookingType = existingOrder.BookingType,
                    BookingFields = existingOrder.BookingFields,
                    Created = DateTime.UtcNow
                };

                _db.Insert(duplicatedOrder);
            }

     
            return Redirect("/allorderpage");
        }

        [HttpPost]
        public IActionResult ChangeStatus(int orderId, string newStatus)
        {
            // Fetch the order
            var order = _db.SingleOrDefault<Order>("WHERE Id = @0", orderId);
            if (order != null)
            {
                // Update status
                order.Status = newStatus;
                _db.Update(order);
            }

            if (newStatus == "Sendt til booker")
            {
                TempData["StatusChanged"] = "Tak for bestillingen! Ordren er nu sendt til booker.";
            }

            return Redirect("/allorderpage");
        }

      


    }


}
