using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
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
            AvailableStaffs = new List<SelectListItem>();
            AvailableCustomers = new List<SelectListItem>();
            AvailableOrderStatus = new List<SelectListItem>();
            PackageItemProcessedDatetimeStatus = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.VendorProductUrl")]
        public string VendorProductUrl { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.ProductSku")]
        public string ProductSku { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.OrderItemId")]
        public string OrderItemId { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ExportVendor.Fields.OrderId")]
        public string OrderId { get; set; }

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

        [NopResourceDisplayName("Admin.Orders.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

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

        [NopResourceDisplayName("Admin.Orders.List.IsPackageItemProcessedDatetime")]
        public bool? IsPackageItemProcessedDatetime { get; set; }
        public List<SelectListItem> PackageItemProcessedDatetimeStatus { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.EstimatedTimeArrivalNew")]
        [UIHint("DateNullable")]
        public DateTime? EstimatedTimeArrivalNew { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.VendorId")]
        public int VendorId { get; set; }

        public List<SelectListItem> VendorItems { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.AssignedByStaffId")]
        public int AssignedByStaffId { get; set; }

        public List<SelectListItem> AvailableStaffs { get; set; }

        public List<SelectListItem> AvailableCustomers { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.OrderItemStatus")]
        public int OrderItemStatusId { get; set; }

        public List<SelectListItem> AvailableOrderStatus { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.IsWeightCostZero")]
        public bool IsWeightCostZero { get; set; }


    }

    public class OrderItemExportVendorModelBasic : BaseNopEntityModel// : OrderModel.OrderItemModel
    {
        public int OrderId { get; set; }
        //public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string VendorName { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public string PictureThumbnailUrl { get; set; }
        public string AttributeInfo { get; set; }
        public string UnitPriceBase { get; set; }
        //public string UnitPriceInclTax { get; set; }
        //public decimal UnitPriceInclTaxValue { get; set; }
        public string SubTotalInclTax { get; set; }
        public decimal SubTotalInclTaxValue { get; set; }
        public string WeightCost { get; set; }
        public decimal WeightCostDec { get; set; }
        public int PackageOrderId { get; set; }
        public string PackageOrderCode { get; set; }
        public DateTime? EstimatedTimeArrival { get; set; }
        public DateTime? PackageItemProcessedDatetime { get; set; }
        //public bool IncludeWeightCost { get; set; }
        public bool IsOrderCheckout { get; set; }
        //public string PrimaryStoreCurrencyCode { get; set; }

        public string ShelfCode { get; set; }

        public int ShelfId { get; set; }
        public int ShelfOrderItemId { get; set; }
        public decimal ItemWeight { get; set; }
        public string TotalWithoutWeightCost { get; set; }
        public decimal UnitWeightCost { get; set; }
        //public PackageOrderModel PackageOrder { get; set; }
        public string CustomerInfo { get; set; }
        public string CustomerLinkFacebook { get; set; }
        public string CustomerLinkShortFacebook { get; set; }
        public int AssignedByCustomerId { get; set; }
        public string CustomerAssignShelfInfo { get; set; }
        public DateTime? DeliveryDateUtc { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Deposit { get; set; }
        public string DepositStr { get; set; }

        //[NopResourceDisplayName("Admin.OrderItem.Status")]
        //public string OrderItemStatus { get; set; }
        //[NopResourceDisplayName("Admin.OrderItem.Status")]
        //public int OrderItemStatusId { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.Note")]
        public string Note { get; set; }

    }
}
