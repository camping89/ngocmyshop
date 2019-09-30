using Nop.Core.Domain.Orders;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment item
    /// </summary>
    public partial class ShipmentManualItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the shipment identifier
        /// </summary>
        public int ShipmentManualId { get; set; }

        /// <summary>
        /// Gets or sets the order item identifier
        /// </summary>
        public int OrderItemId { get; set; }

        public virtual OrderItem OrderItem { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        public int WarehouseId { get; set; }

        public decimal ShippingFee { get; set; }
        
        /// <summary>
        /// Gets the shipment
        /// </summary>
        public virtual ShipmentManual ShipmentManual { get; set; }
    }
}