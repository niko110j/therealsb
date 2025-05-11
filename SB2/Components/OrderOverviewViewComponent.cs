using Microsoft.AspNetCore.Mvc;
using SB2.Models.ViewModels;
using Umbraco.Cms.Infrastructure.Persistence;

public class OrderOverviewViewComponent : ViewComponent
{
    private readonly IUmbracoDatabase _db;
    private readonly IUmbracoDatabaseFactory _databaseFactory;

    public OrderOverviewViewComponent(IUmbracoDatabase db, IUmbracoDatabaseFactory databaseFactory)
    {
        _db = db;
        _databaseFactory = databaseFactory;
    }

    public IViewComponentResult Invoke()
    {
        var db= _databaseFactory.CreateDatabase();
        var orders = db.Fetch<Order>("SELECT * FROM Orders ORDER BY Created DESC");

        var model = new AllOrdersViewModel
        {
            Orders = orders.Select(x => new OrderListItem
            {
                Name = $"Order - {x.Created:yyyyMMddHHmmss}",
                ClientName = x.ClientName,
                ClientEmail = x.ClientEmail,
                SalesPersonName = x.SalespersonName,
                FilledBy = x.FilledBy,
                BookingType = x.BookingType
            }).ToList()
        };

        return View("_OrderOverview", model);
    }
}

