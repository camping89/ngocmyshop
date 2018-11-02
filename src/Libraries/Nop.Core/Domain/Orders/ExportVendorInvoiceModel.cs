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
        public string CustomerInfo { get; set; }
        public string CreatedDate { get; set; }
        public string VendorProductUrl { get; set; }
        public string Sku { get; set; }
        public string ProductInfo { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal TotalWithoutWeightCost { get; set; }
        public decimal WeightCost { get; set; }
        public decimal TotalCost { get; set; }

        public decimal BaseUnitPrice { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal OrderingFee { get; set; }
        public double SaleOff { get; set; }
        public string ETA { get; set; }

        //public string PackageOrderCode { get; set; }
        //public string PackageOrderItemCode { get; set; }

        //public string PackageItemProcessedDatetime { get; set; }

        //public string IsVendorCheckout { get; set; }


    }
}
