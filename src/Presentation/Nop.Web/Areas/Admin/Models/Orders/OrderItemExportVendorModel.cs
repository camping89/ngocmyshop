using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

            IsSetPackageOrderIdStatus = new List<SelectListItem>
            {
                new SelectListItem{ Selected = true,Text = "*"},
                new SelectListItem{ Value = "True",Text = "Đã gán mã kiện hàng"},
                new SelectListItem{ Value = "False",Text = "Chưa gán mã kiện hàng"}
            };

            IsSetShelfIdStatus = new List<SelectListItem>
            {
                new SelectListItem{ Selected = true,Text = "*"},
                new SelectListItem{ Value = "True",Text = "Đã xếp ngăn"},
                new SelectListItem{ Value = "False",Text = "Chưa xếp ngăn"}
            };

            VendorItems = new List<SelectListItem>();
            AvailableCustomers = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.VendorProductUrl")]
        public string VendorProductUrl { get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.OrderId")]
        public int OrderId { get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsOrderCheckout")]
        public bool? IsOrderCheckout { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsOrderCheckoutStatusItems")]
        public List<SelectListItem> IsOrderCheckoutStatusItems { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.HasPackageId")]
        public bool? IsSetPackageOrderId { get; set; }

        public List<SelectListItem> IsSetPackageOrderIdStatus { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsShelfAssigned")]
        public bool? IsShelfAssigned { get; set; }
        public List<SelectListItem> IsSetShelfIdStatus { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsPackageItemProcessed")]
        public bool IsPackageItemProcessed { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.TodayFilter")]
        public bool TodayFilter { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.CustomerPhone")]
        public string CustomerPhone { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PackageOrderCode")]
        public string PackageOrderCode { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PackageOrderIdNew")]
        public string PackageOrderCodeNew { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PackageItemProcessedDatetimeNew")]
        [UIHint("DateNullable")]
        public DateTime? PackageItemProcessedDatetimeNew { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.VendorId")]
        public int VendorId { get; set; }

        public List<SelectListItem> VendorItems { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.AssignedByCustomerId")]
        public int AssignedByNewCustomerId { get; set; }

        public List<SelectListItem> AvailableCustomers { get; set; }


    }

    public class OrderItemExportVendorModelBasic : OrderModel.OrderItemModel
    {
        public string CustomerInfo { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
