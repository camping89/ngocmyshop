﻿using Microsoft.AspNetCore.Mvc.Rendering;
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
            PackageOrderIds = new List<SelectListItem>();

            VendorItems = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.VendorProductUrl")]
        public string VendorProductUrl { get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.OrderId")]
        public int OrderId { get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsOrderCheckout")]
        public bool? IsOrderCheckout { get; set; }

        public List<SelectListItem> IsOrderCheckoutStatusItems { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsPackageItemProcessed")]
        public bool IsPackageItemProcessed { get; set; }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.IsSetPackageItemCode")]
        public bool IsSetPackageItemCode { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.TodayFilter")]
        public bool TodayFilter { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.CustomerPhone")]
        public string CustomerPhone { get; set; }

        public List<SelectListItem> PackageOrderIds { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PackageOrderId")]
        public int PackageOrderId { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PackageOrderIdNew")]
        public int PackageOrderIdNew { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.PackageItemProcessedDatetimeNew")]
        [UIHint("DateNullable")]
        public DateTime? PackageItemProcessedDatetimeNew { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.VendorId")]
        public int VendorId { get; set; }

        public List<SelectListItem> VendorItems { get; set; }
    }

    public class OrderItemExportVendorModelBasic : OrderModel.OrderItemModel
    {
        public string CustomerInfo { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
