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
            Customers = new List<SelectListItem>();
            ShelfOrderItemsStatus = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.CustomerId")]
        public int CustomerId { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.AssignedFromDate")]
        public DateTime? AssignedFromDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.ShelfCode")]
        public string ShelfCode { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Orders.Shelf.Fields.AssignedToDate")]
        public DateTime? AssignedToDate { get; set; }

        public List<SelectListItem> Customers { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.IsActive")]
        public bool? ShelfOrderItemIsActive { get; set; }

        [NopResourceDisplayName("Admin.ShelfOrderItem.IsShelfEmpty")]
        public bool IsShelfEmpty { get; set; }

        public List<SelectListItem> ShelfOrderItemsStatus { get; set; }

    }
}
