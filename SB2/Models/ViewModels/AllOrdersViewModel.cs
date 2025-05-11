using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Models;

namespace SB2.Models.ViewModels
{
    public class AllOrdersViewModel : ContentModel
    {
        public AllOrdersViewModel(IPublishedContent? content) : base(content)
        { }
        public List<OrderListItem> Orders { get; set; } = new(); 
    }  

}
