using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public class PackageOrderModel : BaseNopEntityModel
    {
        public PackageOrderModel()
        {
            OrderItems = new List<OrderModel.OrderItemModel>();
        }
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.OrderPackages.Fields.PackageCode")]
        public string PackageCode { get; set; }
        [NopResourceDisplayName("Admin.OrderPackages.Fields.PackageName")]
        public string PackageName { get; set; }

        [NopResourceDisplayName("Admin.OrderPackages.Fields.ArrivalDatetime")]
        public DateTime? ArrivalDatetime { get; set; }

        [NopResourceDisplayName("Admin.OrderPackages.Fields.PackageProcessedDatetime")]
        public DateTime? PackageProcessedDatetime { get; set; }

        [NopResourceDisplayName("Admin.OrderPackages.Fields.IsShipped")]
        public bool IsShipped { get; set; }

        public IList<OrderModel.OrderItemModel> OrderItems { get; set; }
    }
}
