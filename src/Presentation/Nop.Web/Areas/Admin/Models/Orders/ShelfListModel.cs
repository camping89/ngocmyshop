using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public class ShelfListModel
    {
        public ShelfListModel()
        {
            OrderItemsStatus = new List<SelectListItem>();
            CustomerNotifiedStatus = new List<SelectListItem>();
            ShelfNoteStatus = new List<SelectListItem>();
            PackageItemProcessedDatetimeStatus = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.CustomerId")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.CustomerPhone")]
        public string CustomerPhone { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.CustomerFacebook")]
        public string CustomerFacebook { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.AssignedFromDate")]
        public DateTime? AssignedFromDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.AssignedToDate")]
        public DateTime? AssignedToDate { get; set; }
        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.AssignedOrderItemFromDate")]
        public DateTime? AssignedOrderItemFromDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.AssignedOrderItemToDate")]
        public DateTime? AssignedOrderItemToDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.ShippedFromDate")]
        public DateTime? ShippedFromDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.ShippedToDate")]
        public DateTime? ShippedToDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.ShelfCode")]
        public string ShelfCode { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.InActive")]
        public bool InActive { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.IsCustomerNotified")]
        public bool? IsCustomerNotified { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.IsShelfEmpty")]
        public bool IsShelfEmpty { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.IsEmptyAssignedShelf")]
        public bool IsEmptyAssignedShelf { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ShipTodayFilter")]
        public bool ShipTodayFilter { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.ShelfNoteId")]
        public int? ShelfNoteId { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.IsPackageItemProcessedDatetime")]
        public bool? IsPackageItemProcessedDatetime { get; set; }

        [NopResourceDisplayName("Admin.OrderItem.IsAscSortedAssignedDate")]
        public bool IsAscSortedAssignedDate { get; set; }

        public List<SelectListItem> PackageItemProcessedDatetimeStatus { get; set; }
        public List<SelectListItem> OrderItemsStatus { get; set; }
        public List<SelectListItem> CustomerNotifiedStatus { get; set; }
        public List<SelectListItem> ShelfNoteStatus { get; set; }

        public bool IsAdmin { get; set; }

    }
}
