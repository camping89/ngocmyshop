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
            ShelfOrderItemsStatus = new List<SelectListItem>();
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

        [NopResourceDisplayName("Admin.ShelfOrderItem.IsActive")]
        public bool? ShelfOrderItemIsActive { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.IsCustomerNotified")]
        public bool? IsCustomerNotified { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.IsShelfEmpty")]
        public bool IsShelfEmpty { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.IsEmptyAssignedShelf")]
        public bool IsEmptyAssignedShelf { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.ShipTodayFilter")]
        public bool ShipTodayFilter { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.ShelfNoteId")]
        public int? ShelfNoteId { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.IsPackageItemProcessedDatetime")]
        public bool? IsPackageItemProcessedDatetime { get; set; }

        public List<SelectListItem> PackageItemProcessedDatetimeStatus { get; set; }
        public List<SelectListItem> ShelfOrderItemsStatus { get; set; }
        public List<SelectListItem> CustomerNotifiedStatus { get; set; }
        public List<SelectListItem> ShelfNoteStatus { get; set; }

        public bool IsAdmin { get; set; }

    }
}
