﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AddressModel = Nop.Web.Areas.Admin.Models.Common.AddressModel;

namespace Nop.Web.Areas.Admin.Models.Orders
{

    public partial class OrderModelBasic : BaseNopEntityModel
    {

        [NopResourceDisplayName("Admin.Orders.Fields.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotal")]
        public string OrderTotal { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public string OrderStatus { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public int OrderStatusId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.PaymentStatus")]
        public string PaymentStatus { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.ShippingStatus")]
        public string ShippingStatus { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerFullName")]
        public string CustomerFullName { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerPhone")]
        public string CustomerPhone { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerLinkFacebook")]
        public string CustomerLinkFacebook { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.WeightCost")]
        public string WeightCost { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.AdminNote")]
        public string AdminNote { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotalWithoutWeightCost")]
        public string OrderTotalWithoutWeightCost { get; set; }
    }
    public partial class OrderModel : BaseNopEntityModel
    {
        public OrderModel()
        {
            CustomValues = new Dictionary<string, object>();
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<OrderItemModel>();
            UsedDiscounts = new List<UsedDiscountModel>();
            PackageOrderModels = new List<PackageOrderModel>();
        }

        public bool IsLoggedInAsVendor { get; set; }

        //identifiers
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderGuid")]
        public Guid OrderGuid { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        //store
        [NopResourceDisplayName("Admin.Orders.Fields.Store")]
        public string StoreName { get; set; }

        //customer info
        [NopResourceDisplayName("Admin.Orders.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Customer")]
        public string CustomerInfo { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }
        public string CustomerFullName { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerIP")]
        public string CustomerIp { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerPhone")]
        public string CustomerPhone { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomValues")]
        public Dictionary<string, object> CustomValues { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.Affiliate")]
        public int AffiliateId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Affiliate")]
        public string AffiliateName { get; set; }


        [NopResourceDisplayName("Admin.Orders.Fields.AdminNote")]
        public string AdminNote { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.WeightCost")]
        public string WeightCost { get; set; }


        //Used discounts
        [NopResourceDisplayName("Admin.Orders.Fields.UsedDiscounts")]
        public IList<UsedDiscountModel> UsedDiscounts { get; set; }

        //totals
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }
        public TaxDisplayType TaxDisplayType { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubtotalInclTax")]
        public string OrderSubtotalInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubtotalExclTax")]
        public string OrderSubtotalExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubTotalDiscountInclTax")]
        public string OrderSubTotalDiscountInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubTotalDiscountExclTax")]
        public string OrderSubTotalDiscountExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderShippingInclTax")]
        public string OrderShippingInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderShippingExclTax")]
        public string OrderShippingExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentMethodAdditionalFeeInclTax")]
        public string PaymentMethodAdditionalFeeInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentMethodAdditionalFeeExclTax")]
        public string PaymentMethodAdditionalFeeExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Tax")]
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotalDiscount")]
        public string OrderTotalDiscount { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.RedeemedRewardPoints")]
        public int RedeemedRewardPoints { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.RedeemedRewardPoints")]
        public string RedeemedRewardPointsAmount { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotal")]
        public string OrderTotal { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotalWithoutWeightCost")]
        public string OrderTotalWithoutWeightCost { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.RefundedAmount")]
        public string RefundedAmount { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Profit")]
        public string Profit { get; set; }

        //edit totals
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubtotal")]
        public decimal OrderSubtotalInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubtotal")]
        public decimal OrderSubtotalExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubTotalDiscount")]
        public decimal OrderSubTotalDiscountInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubTotalDiscount")]
        public decimal OrderSubTotalDiscountExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderShipping")]
        public decimal OrderShippingInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderShipping")]
        public decimal OrderShippingExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.Tax")]
        public decimal TaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.TaxRates")]
        public string TaxRatesValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderTotalDiscount")]
        public decimal OrderTotalDiscountValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderTotal")]
        public decimal OrderTotalValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderCurrentSubtotal")]
        public string OrderCurrentSubtotal { get; set; }

        //associated recurring payment id
        [NopResourceDisplayName("Admin.Orders.Fields.RecurringPayment")]
        public int RecurringPaymentId { get; set; }

        //order status
        [NopResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public string OrderStatus { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public int OrderStatusId { get; set; }

        //payment info
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentStatus")]
        public string PaymentStatus { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentStatus")]
        public int PaymentStatusId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; }

        //credit card info
        public bool AllowStoringCreditCardNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardType")]
        public string CardType { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardName")]
        public string CardName { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardNumber")]
        public string CardNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardCVV2")]
        public string CardCvv2 { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardExpirationMonth")]
        public string CardExpirationMonth { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardExpirationYear")]
        public string CardExpirationYear { get; set; }

        //misc payment info
        [NopResourceDisplayName("Admin.Orders.Fields.AuthorizationTransactionID")]
        public string AuthorizationTransactionId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CaptureTransactionID")]
        public string CaptureTransactionId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.SubscriptionTransactionID")]
        public string SubscriptionTransactionId { get; set; }

        //shipping info
        public bool IsShippable { get; set; }
        public bool PickUpInStore { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PickupAddress")]
        public AddressModel PickupAddress { get; set; }
        public string PickupAddressGoogleMapsUrl { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingStatus")]
        public string ShippingStatus { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingStatus")]
        public int ShippingStatusId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingAddress")]
        public AddressModel ShippingAddress { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }
        public List<ShippingMethod> ShippingMethodList { get; set; }
        public string ShippingAddressGoogleMapsUrl { get; set; }
        public bool CanAddNewShipments { get; set; }

        //billing info
        [NopResourceDisplayName("Admin.Orders.Fields.BillingAddress")]
        public AddressModel BillingAddress { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.VatNumber")]
        public string VatNumber { get; set; }

        //gift cards
        public IList<GiftCard> GiftCards { get; set; }

        //items
        public bool HasDownloadableProducts { get; set; }
        public IList<OrderItemModel> Items { get; set; }
        public IList<PackageOrderModel> PackageOrderModels { get; set; }

        //creation date
        [NopResourceDisplayName("Admin.Orders.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //checkout attributes
        public string CheckoutAttributeInfo { get; set; }

        public string ProductAttributeInfo { get; set; }

        //order notes
        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.DisplayToCustomer")]
        public bool AddOrderNoteDisplayToCustomer { get; set; }
        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.Note")]
        public string AddOrderNoteMessage { get; set; }
        public bool AddOrderNoteHasDownload { get; set; }
        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.Download")]
        [UIHint("Download")]
        public int AddOrderNoteDownloadId { get; set; }

        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.IsOrderCheckout")]
        public bool IsOrderCheckout { get; set; }

        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.OrderCheckoutDatetime")]
        public DateTime? OrderCheckoutDatetime { get; set; }

        //refund info
        [NopResourceDisplayName("Admin.Orders.Fields.PartialRefund.AmountToRefund")]
        public decimal AmountToRefund { get; set; }
        public decimal MaxAmountToRefund { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }

        //workflow info
        public bool CanCancelOrder { get; set; }
        public bool CanCapture { get; set; }
        public bool CanMarkOrderAsPaid { get; set; }
        public bool CanRefund { get; set; }
        public bool CanRefundOffline { get; set; }
        public bool CanPartiallyRefund { get; set; }
        public bool CanPartiallyRefundOffline { get; set; }
        public bool CanVoid { get; set; }
        public bool CanVoidOffline { get; set; }

        #region Nested Classes
        public partial class OrderItemModelBasic
        {
            public int OrderId { get; set; }
            public int Id { get; set; }
            public int ProductId { get; set; }
            public string Sku { get; set; }
            public string UnitPriceBase { get; set; }
            public string ProductName { get; set; }
            public int PackageOrderId { get; set; }
            public string PackageOrderCode { get; set; }
            public PackageOrderModel PackageOrder { get; set; }
            public string EstimatedTimeArrival { get; set; }
            public string PackageItemProcessedDatetime { get; set; }
            public bool IncludeWeightCost { get; set; }
            public bool IsOrderCheckout { get; set; }
            public string WeightCost { get; set; }
            public decimal WeightCostDec { get; set; }
            public decimal UnitWeightCost { get; set; }
            public decimal ItemWeight { get; set; }
            public string TotalWithoutWeightCost { get; set; }

            public string SubTotalInclTax { get; set; }
            public string SubTotalExclTax { get; set; }
            public decimal SubTotalInclTaxValue { get; set; }
            public decimal SubTotalExclTaxValue { get; set; }


            public string ShelfCode { get; set; }

            public int ShelfId { get; set; }
            public int ShelfOrderItemId { get; set; }
            public bool ShelfOrderItemIsActive { get; set; }

            public string AttributeInfo { get; set; }
            public string PictureThumbnailUrl { get; set; }
            public DateTime? DeliveryDateUtc { get; set; }

            public string PrimaryStoreCurrencyCode { get; set; }
            public bool OutOfStock { get; set; }
            public decimal Deposit { get; set; }

        }
        public partial class OrderItemModel : BaseNopEntityModel
        {
            public OrderItemModel()
            {
                PurchasedGiftCardIds = new List<int>();
                ReturnRequests = new List<ReturnRequestBriefModel>();
            }
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string VendorName { get; set; }
            public string Sku { get; set; }

            public string UnitPriceBase { get; set; }
            public string PictureThumbnailUrl { get; set; }

            public string UnitPriceInclTax { get; set; }
            public string UnitPriceExclTax { get; set; }
            public decimal UnitPriceInclTaxValue { get; set; }
            public decimal UnitPriceExclTaxValue { get; set; }

            public int Quantity { get; set; }

            public string DiscountInclTax { get; set; }
            public string DiscountExclTax { get; set; }
            public decimal DiscountInclTaxValue { get; set; }
            public decimal DiscountExclTaxValue { get; set; }

            public string SubTotalInclTax { get; set; }
            public string SubTotalExclTax { get; set; }
            public decimal SubTotalInclTaxValue { get; set; }
            public decimal SubTotalExclTaxValue { get; set; }

            public string AttributeInfo { get; set; }
            public string RecurringInfo { get; set; }
            public string RentalInfo { get; set; }
            public IList<ReturnRequestBriefModel> ReturnRequests { get; set; }
            public IList<int> PurchasedGiftCardIds { get; set; }

            public bool IsDownload { get; set; }
            public int DownloadCount { get; set; }
            public DownloadActivationType DownloadActivationType { get; set; }
            public bool IsDownloadActivated { get; set; }
            public Guid LicenseDownloadGuid { get; set; }
            public decimal ItemWeight { get; set; }
            public decimal UnitWeightCost { get; set; }
            public string WeightCost { get; set; }
            public decimal WeightCostDec { get; set; }
            public string TotalWithoutWeightCost { get; set; }
            public int PackageOrderId { get; set; }
            public string PackageOrderCode { get; set; }

            public PackageOrderModel PackageOrder { get; set; }


            public int? AssignedByCustomerId { get; set; }
            public Customer AssignedByCustomer { get; set; }
            public string CustomerAssignShelfInfo { get; set; }
            public DateTime? EstimatedTimeArrival { get; set; }
            public DateTime? PackageItemProcessedDatetime { get; set; }

            public bool IncludeWeightCost { get; set; }
            public bool IsOrderCheckout { get; set; }
            public bool OutOfStock { get; set; }
            public string PrimaryStoreCurrencyCode { get; set; }

            public string ShelfCode { get; set; }

            public int ShelfId { get; set; }
            public int ShelfOrderItemId { get; set; }
            public bool ShelfOrderItemIsActive { get; set; }

            public DateTime? DeliveryDateUtc { get; set; }

            public decimal Deposit { get; set; }

            #region Nested Classes

            public partial class ReturnRequestBriefModel : BaseNopEntityModel
            {
                public string CustomNumber { get; set; }
            }

            #endregion
        }

        public partial class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Orders.Fields.GiftCardInfo")]
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class OrderNote : BaseNopEntityModel
        {
            public int OrderId { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.DisplayToCustomer")]
            public bool DisplayToCustomer { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.Note")]
            public string Note { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.Download")]
            public int DownloadId { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.Download")]
            public Guid DownloadGuid { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class UploadLicenseModel : BaseNopModel
        {
            public int OrderId { get; set; }

            public int OrderItemId { get; set; }

            [UIHint("Download")]
            public int LicenseDownloadId { get; set; }

        }

        public partial class AddOrderProductModel : BaseNopModel
        {
            public AddOrderProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableProductTypes = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductType")]
            public int SearchProductTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableProductTypes { get; set; }

            public int OrderId { get; set; }

            #region Nested classes

            public partial class ProductModel : BaseNopEntityModel
            {
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Name")]
                public string Name { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.SKU")]
                public string Sku { get; set; }

                public string UrlImage { get; set; }
            }

            public partial class ProductDetailsModel : BaseNopModel
            {
                public ProductDetailsModel()
                {
                    ProductAttributes = new List<ProductAttributeModel>();
                    GiftCard = new GiftCardModel();
                    Warnings = new List<string>();
                    CurrencySelectorModel = new CurrencySelectorModel();
                }

                public int ProductId { get; set; }

                public int OrderId { get; set; }

                public ProductType ProductType { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Sku")]
                public string Sku { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.VendorProductUrl")]
                public string LinkProduct { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Image")]
                public string ImageUrl { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Name")]
                public string Name { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.UnitPriceInclTax")]
                public decimal UnitPriceInclTax { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.UnitPriceExclTax")]
                public decimal UnitPriceExclTax { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Quantity")]
                public int Quantity { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.SubTotalInclTax")]
                public decimal SubTotalInclTax { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.SubTotalExclTax")]
                public decimal SubTotalExclTax { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.BaseUnitPrice")]
                public decimal BaseUnitPrice { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.ExchangeRate")]
                public decimal ExchangeRate { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.OrderingFee")]
                public decimal OrderingFee { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.SaleOffPercent")]
                public double SaleOffPercent { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.CurrencyId")]
                public int CurrencyId { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Currency")]
                public Currency Currency { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.CurrencyCurrent")]
                public Currency CurrencyCurrent { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Weight")]
                public string Weight { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.WeightCost")]
                public decimal WeightCost { get; set; }

                //product attributes
                public IList<ProductAttributeModel> ProductAttributes { get; set; }

                public CurrencySelectorModel CurrencySelectorModel { get; set; }
                //gift card info
                public GiftCardModel GiftCard { get; set; }
                //rental
                public bool IsRental { get; set; }

                public List<string> Warnings { get; set; }

                /// <summary>
                /// A value indicating whether this attribute depends on some other attribute
                /// </summary>
                public bool HasCondition { get; set; }

                public bool AutoUpdateOrderTotals { get; set; }
            }

            public partial class ProductAttributeModel : BaseNopEntityModel
            {
                public ProductAttributeModel()
                {
                    Values = new List<ProductAttributeValueModel>();
                }

                public int ProductAttributeId { get; set; }

                public string Name { get; set; }

                public string TextPrompt { get; set; }

                public bool IsRequired { get; set; }

                public bool HasCondition { get; set; }

                /// <summary>
                /// Allowed file extensions for customer uploaded files
                /// </summary>
                public IList<string> AllowedFileExtensions { get; set; }

                public AttributeControlType AttributeControlType { get; set; }

                public IList<ProductAttributeValueModel> Values { get; set; }
            }

            public partial class ProductAttributeValueModel : BaseNopEntityModel
            {
                public string Name { get; set; }

                public bool IsPreSelected { get; set; }

                public string PriceAdjustment { get; set; }

                public decimal PriceAdjustmentValue { get; set; }

                public bool CustomerEntersQty { get; set; }

                public int Quantity { get; set; }
            }

            public partial class GiftCardModel : BaseNopModel
            {
                public bool IsGiftCard { get; set; }

                [NopResourceDisplayName("Admin.GiftCards.Fields.RecipientName")]
                public string RecipientName { get; set; }
                [DataType(DataType.EmailAddress)]
                [NopResourceDisplayName("Admin.GiftCards.Fields.RecipientEmail")]
                public string RecipientEmail { get; set; }
                [NopResourceDisplayName("Admin.GiftCards.Fields.SenderName")]
                public string SenderName { get; set; }
                [DataType(DataType.EmailAddress)]
                [NopResourceDisplayName("Admin.GiftCards.Fields.SenderEmail")]
                public string SenderEmail { get; set; }
                [NopResourceDisplayName("Admin.GiftCards.Fields.Message")]
                public string Message { get; set; }

                public GiftCardType GiftCardType { get; set; }
            }

            #endregion
        }

        public partial class UsedDiscountModel : BaseNopModel
        {
            public int DiscountId { get; set; }
            public string DiscountName { get; set; }
        }

        #endregion
    }

    public partial class OrderAggreratorModel : BaseNopModel
    {
        //aggergator properties
        public string aggregatorprofit { get; set; }
        public string aggregatorshipping { get; set; }
        public string aggregatortax { get; set; }
        public string aggregatortotal { get; set; }
    }


}