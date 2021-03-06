using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment
    /// </summary>
    public partial class ShipmentManual : BaseEntity
    {
        private ICollection<ShipmentManualItem> _shipmentManualItems;


        /// <summary>
        /// Gets or sets the tracking number of this shipment
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the total weight of this shipment
        /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        public decimal? TotalWeight { get; set; }
        public decimal TotalShippingFee { get; set; }
        public bool HasShippingFee { get; set; }

        /// <summary>
        /// Gets or sets the shipped date and time
        /// </summary>
        public DateTime? ShippedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the delivery date and time
        /// </summary>
        public DateTime? DeliveryDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }
        /// <summary>
        /// Admin note 
        /// </summary>
        public string ShipmentNote { get; set; }
        /// <summary>
        /// Gets or sets the entity creation date
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        public string ShelfCode { get; set; }
        public decimal Total { get; set; }

        public string BagId { get; set; }

        public int ShipperId { get; set; }

        public virtual Customer Shipper { get; set; }
        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }

        public int? ShippingAddressId { get; set; }
        public decimal Deposit { get; set; }
        public virtual Address ShippingAddress { get; set; }
        
        public int? UsedRewardPointOrderItemId { get; set; }
        /// <summary>
        /// Gets or sets the reward points history record (spent by a customer when order item shipped.)
        /// </summary>
        public virtual RewardPointsHistoryOrderItem RedeemedRewardPointsOrderItemEntry { get; set; }

        /// <summary>
        /// Gets or sets the shipment items
        /// </summary>
        public virtual ICollection<ShipmentManualItem> ShipmentManualItems
        {
            get { return _shipmentManualItems ?? (_shipmentManualItems = new List<ShipmentManualItem>()); }
            protected set { _shipmentManualItems = value; }
        }
    }
}