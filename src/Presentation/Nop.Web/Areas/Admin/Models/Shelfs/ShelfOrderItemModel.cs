using System;

namespace Nop.Web.Areas.Admin.Models.Shelfs
{
    public class OrderItemInShelfModel
    {
        public int OrderId { get; set; }

        public int Id { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }

        public string AttributeInfo { get; set; }
        public string PictureThumbnailUrl { get; set; }
        public string DeliveryDateUtc { get; set; }


        public string PrimaryStoreCurrencyCode { get; set; }
        public string DepositStr { get; set; }
        public string OrderItemStatus { get; set; }
        public int OrderItemStatusId { get; set; }
        public string Note { get; set; }
        public string VendorName { get; set; }
        public string ShelfCode { get; set; }
        public string WeightCost { get; set; }
        public decimal WeightCostDec { get; set; }
        public string TotalWithoutWeightCost { get; set; }
        public string SubTotalInclTax { get; set; }

        public int PackageOrderId { get; set; }
        public string PackageOrderCode { get; set; }

        public DateTime? AssignedShelfDate { get; set; }

        public bool ExistShipment { get; set; }
    }
}
