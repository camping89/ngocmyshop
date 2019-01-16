namespace Nop.Core.Domain.Shipping
{
    public class ShipmentExportModel
    {
        public ShipmentExportModel()
        {
            CustomerPhone = string.Empty;
            CustomerFacebookUrl = string.Empty;
            CustomerName = string.Empty;
        }
        public string ShipmentId { get; set; }

        public string OrderId { get; set; }
        public string OrderItemId { get; set; }
        public string BagId { get; set; }
        public string TrackingNumber { get; set; }
        public string ProductInfo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerFacebookUrl { get; set; }
        public string ShipperInfo { get; set; }
        public string DeliveryDate { get; set; }
        public string ShippedDate { get; set; }
        public string Note { get; set; }
        public decimal Deposit { get; set; }
        public decimal TotalShippingFee { get; set; }
    }
}
