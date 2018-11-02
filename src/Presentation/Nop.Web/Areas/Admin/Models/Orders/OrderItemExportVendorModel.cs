using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public class OrderItemExportVendorModel
    {
        public OrderItemExportVendorModel()
        {
            IsOrderCheckoutStatusItems = new List<SelectListItem>
            {
                new SelectListItem{ Selected = true,Text = "*"},
                new SelectListItem{ Value = "True",Text = "Đã xuất đơn"},
                new SelectListItem{ Value = "False",Text = "Chưa xuất đơn"}
            };
        }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.VendorProductUrl")]
        public string VendorProductUrl { get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.OrderId")]
        public int OrderId { get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsOrderCheckout")]
        public bool? IsOrderCheckout { get; set; }
        
        public List<SelectListItem> IsOrderCheckoutStatusItems { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsPackageItemProcessed")]
        public bool IsPackageItemProcessed {get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsSetPackageItemCode")]
        public bool IsSetPackageItemCode {get; set; }

        [NopResourceDisplayName("Admin.Orders.List.TodayFilter")]
        public bool TodayFilter { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.CustomerPhone")]
        public string CustomerPhone { get; set; }



    }

    public class OrderItemExportVendorModelBasic : OrderModel.OrderItemModel
    {
        public string CustomerInfo{get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
