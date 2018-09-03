using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class OrderCreateOrUpdateModel
    {
        public OrderCreateOrUpdateModel()
        {
            ProductListModel = new ProductListModel();
        }
        [NopResourceDisplayName("Admin.Catalog.Orders.CreateOrder.CustomerName")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Orders.CreateOrder.CustomerPhone")]
        public string CustomerPhone { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Orders.CreateOrder.CustomerFacebook")]
        public string CustomerFacebook { get; set; }
        public string CustomerFullName { get; set; }
        public ProductListModel ProductListModel { get; set; }

        public Web.Models.ShoppingCart.ShoppingCartModel ShoppingCartModel { get; set; }
        public bool SetUpdateOrder { get; set; }
    }
}
