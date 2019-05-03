using System;

namespace Nop.Core.Domain.Orders
{
    public class ExportVendorInvoiceModel
    {
        public string CustomerInfo { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public string PriceBase { get; set; }
        public double DiscountPercent { get; set; }
        public string Price { get; set; }
        public string Total { get; set; }
        public string VendorProductUrl { get; set; }
        public string AdminNote { get; set; }

    }
    public class ExportOrderBasicModel
    {
        public string OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string OrderPaymentStatus { get; set; }
        public string CustomerInfo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TotalWithoutWeightCost { get; set; }
        public string WeightCost { get; set; }
        public string Total { get; set; }
        public string AdminNote { get; set; }

    }

    public class ExportVendorInvoiceItemModel
    {
        public string OrderId { get; set; }
        public string OrderItemId { get; set; }
        public string AssignedByUser { get; set; }
        public string CustomerInfo { get; set; }
        public string OrderDate { get; set; }
        public string VendorProductUrl { get; set; }
        public string Sku { get; set; }
        public string ProductInfo { get; set; }
        public string ProductSize { get; set; }
        public string ProductColor { get; set; }
        public string ProductAttributeInfo { get; set; }
        public string VendorName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal TotalWithoutWeightCost { get; set; }
        public decimal WeightCost { get; set; }
        public decimal TotalCost { get; set; }
        public string CurrencyCode { get; set; }
        public string UnitWeightCost { get; set; }
        public string Weight { get; set; }

        public decimal BaseUnitPrice { get; set; }
        public string Deposit { get; set; }
        public string ShelfCode { get; set; }
        public string DeliveryDateUtc { get; set; }
        public string OrderItemStatus { get; set; }
        public string IsOrderCheckout { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal OrderingFee { get; set; }
        public double SaleOff { get; set; }
        public string ETA { get; set; }
        public string Note { get; set; }
        public string PackageOrderCode { get; set; }
        //public string PackageOrderItemCode { get; set; }

        public string PackageItemProcessedDatetime { get; set; }

        //public string IsVendorCheckout { get; set; }


    }
}
