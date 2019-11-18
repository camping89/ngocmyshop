using Nop.Core.Domain.Customers;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    public partial class ShipmentManualModel : BaseNopEntityModel
    {
        public ShipmentManualModel()
        {

        }

        [NopResourceDisplayName("Admin.Orders.Shipments.ID")]
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.TotalWeight")]
        public string TotalWeight { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.TotalShippingFee")]
        public decimal TotalShippingFee { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.HasShippingFee")]
        public bool HasShippingFee { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.TotalOrderFee")]
        public string TotalOrderFee { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.TotalWithoutDeposit")]
        public string TotalWithoutDeposit { get; set; }
        public decimal TotalOrderFeeDecimal { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.TrackingNumber")]
        public string TrackingNumber { get; set; }
        public string TrackingNumberUrl { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShipmentAddress")]
        public string ShipmentAddress { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShipmentCity")]
        public string ShipmentCity { get; set; }
        public string ShipmentCityId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShipmentDistrict")]
        public string ShipmentDistrict { get; set; }
        public string ShipmentDistrictId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShipmentWard")]
        public string ShipmentWard { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.ShippedDate")]
        public string ShippedDate { get; set; }

        public bool CanShip { get; set; }

        [UIHint("DateNullable")]
        public DateTime? ShippedDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ReceivedDate")]
        public string DeliveryDate { get; set; }
        public bool CanDeliver { get; set; }
        [UIHint("DateNullable")]
        public DateTime? DeliveryDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.BagId")]
        public string BagId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.Customer")]
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShipperFullName")]
        public string CustomerFullName { get; set; }
        public string CustomerLinkFacebook { get; set; }
        public string CustomerLinkFacebookShort { get; set; }
        public string CustomerPhone { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.Shipper")]
        public int? ShipperId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.ShipperFullName")]
        public string ShipperFullName { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShipmentNote")]
        public string ShipmentNote { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.Deposit")]
        public decimal Deposit { get; set; }
        public string DepositStr { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShelfCode")]
        public string ShelfCode { get; set; }


        [NopResourceDisplayName("Admin.Orders.Shipments.RedeemedRewardPoints")]
        public string RedeemedRewardPoints { get; set; }


        [NopResourceDisplayName("Admin.Orders.Shipments.RedeemedRewardPoints")]
        public string RedeemedRewardPointAmount { get; set; }


        [NopResourceDisplayName("Admin.Orders.Fields.ShippingAddress")]
        public AddressModel ShippingAddress { get; set; }

        public bool AllowDelete{get; set; }
        public List<ShipmentManualItemModel> Items { get; set; }

        public partial class ShipmentManualItemModel : BaseNopEntityModel
        {
            public ShipmentManualItemModel()
            {
            }

            public int OrderItemId { get; set; }
            public string OrderItemNumber { get; set; }
            public int ProductId { get; set; }
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.ProductName")]
            public string ProductName { get; set; }
            public string Sku { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }
            public bool ShipSeparately { get; set; }

            public string ImageUrl { get; set; }
            //weight of one item (product)
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.ItemWeight")]
            public string ItemWeight { get; set; }
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.ItemDimensions")]
            public string ItemDimensions { get; set; }
            public string OrderItemFee { get; set; }
            public int QuantityToAdd { get; set; }
            public int QuantityOrdered { get; set; }
            [NopResourceDisplayName("Admin.Orders.Shipments.Products.QtyShipped")]
            public int QuantityInThisShipment { get; set; }
            public int QuantityInAllShipments { get; set; }

            public string ShippedFromWarehouse { get; set; }
            public decimal ShippingFee { get; set; }
            public string ShippingFeeStr { get; set; }

        }
    }


}