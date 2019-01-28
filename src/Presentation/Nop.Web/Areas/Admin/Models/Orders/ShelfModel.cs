using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public class ShelfModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Shipment.Shelf.ShelfCode")]
        public string ShelfCode { get; set; }

        [NopResourceDisplayName("Admin.Shipment.Shelf.Customer")]
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Shipment.Shelf.AssignedDate")]
        public string AssignedDate { get; set; }

        [NopResourceDisplayName("Admin.Shipment.Shelf.CustomerFullName")]
        public string CustomerFullName { get; set; }

        [NopResourceDisplayName("Admin.Shipment.Shelf.CustomerFullName")]
        public string CustomerPhone { get; set; }

        [NopResourceDisplayName("Admin.Shipment.Shelf.CustomerLinkFacebook")]
        public string CustomerLinkFacebook { get; set; }

        public List<SelectListItem> Customers { get; set; }
    }

    public class ShelfOrderItemModel : BaseNopEntityModel
    {
        public int ShelfId { get; set; }
        public int OrderItemId { get; set; }

        public DateTime AssignedDate { get; set; }
        public bool IsActived { get; set; }
        public virtual ShelfModel ShelfModel { get; set; }
        public virtual CustomerModel CustomerModel { get; set; }
        public virtual OrderModel.OrderItemModel OrderItemModel { get; set; }
    }
}
