using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SelectListItem = Microsoft.AspNetCore.Mvc.Rendering.SelectListItem;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class ShipmentListModel : BaseNopModel
    {
        public ShipmentListModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            AvailableShippers = new List<SelectListItem>();
            AvailableShippersForSearch = new List<SelectListItem>();
            AvailableCities = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.ShipperPhoneNumber")]
        public string ShipperPhoneNumber { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.TodayFilter")]
        public bool TodayFilter { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.ShippedStartDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.ShippedEndDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? EndDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.TrackingNumber")]
        public string TrackingNumber { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.TrackingNumber")]
        public string TrackingNumberNew { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.BagId")]
        public string BagIdNew { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.Country")]
        public int CountryId { get; set; }

        public IList<SelectListItem> AvailableStates { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.StateProvince")]
        public int StateProvinceId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.City")]
        public string City { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.District")]
        public string District { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.LoadNotShipped")]
        public bool LoadNotShipped { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.Warehouse")]
        public int WarehouseId { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.OrderId")]
        public int OrderId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.OrderItemId")]
        public int OrderItemId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.List.CustomerNew")]
        public int CustomerNewId { get; set; }
        public IList<SelectListItem> AvailableShippers { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.SearchShipperId")]
        public int SearchShipperId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.CustomerId")]
        public int CustomerId { get; set; }
        public IList<SelectListItem> AvailableShippersForSearch { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.ExceptCity")]
        public bool ExceptCity { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.List.IsShipDateEmpty")]
        public bool IsNotSetShippedDate { get; set; }

    }

}