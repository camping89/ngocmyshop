using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public class PackageOrderListModel
    {
        [NopResourceDisplayName("Admin.PackageOrder.List.SearchCode")]
        public string Code { get; set; }

        [NopResourceDisplayName("Admin.PackageOrder.List.SearchName")]
        public string Name { get; set; }
    }
}
