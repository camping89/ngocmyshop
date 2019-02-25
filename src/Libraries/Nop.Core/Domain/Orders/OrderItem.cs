using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public partial class OrderItem : BaseEntity
    {
        public OrderItem()
        {
            OrderItemStatus = OrderItemStatus.Available;
        }
        private ICollection<GiftCard> _associatedGiftCards;

        /// <summary>
        /// Gets or sets the order item identifier
        /// </summary>
        public Guid OrderItemGuid { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (incl tax)
        /// </summary>
        public decimal UnitPriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (excl tax)
        /// </summary>
        public decimal UnitPriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (incl tax)
        /// </summary>
        public decimal PriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (excl tax)
        /// </summary>
        public decimal PriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (incl tax)
        /// </summary>
        public decimal DiscountAmountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (excl tax)
        /// </summary>
        public decimal DiscountAmountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the original cost of this order item (when an order was placed), qty 1
        /// </summary>
        public decimal OriginalProductCost { get; set; }

        /// <summary>
        /// Gets or sets the attribute description
        /// </summary>
        public string AttributeDescription { get; set; }

        /// <summary>
        /// Gets or sets the product attributes in XML format
        /// </summary>
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download is activated
        /// </summary>
        public bool IsDownloadActivated { get; set; }

        /// <summary>
        /// Gets or sets a license download identifier (in case this is a downloadable product)
        /// </summary>
        public int? LicenseDownloadId { get; set; }

        /// <summary>
        /// Gets or sets the total weight of one item
        /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        public decimal? ItemWeight { get; set; }

        /// <summary>
        /// Gets or sets the rental product start date (null if it's not a rental product)
        /// </summary>
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental product end date (null if it's not a rental product)
        /// </summary>
        public DateTime? RentalEndDateUtc { get; set; }


        public decimal UnitPriceUsd { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal OrderingFee { get; set; }
        public double SaleOffPercent { get; set; }
        public int CurrencyId { get; set; }
        public decimal? UnitWeightCost { get; set; }
        public decimal WeightCost { get; set; }

        public int? PackageOrderId { get; set; }
        public virtual PackageOrder PackageOrder { get; set; }


        public int? AssignedByCustomerId { get; set; }
        public virtual Customer AssignedByCustomer { get; set; }

        public DateTime? EstimatedTimeArrival { get; set; }
        public DateTime? PackageItemProcessedDatetime { get; set; }

        public DateTime? DeliveryDateUtc { get; set; }


        public bool IncludeWeightCost { get; set; }
        public bool IsOrderCheckout { get; set; }

        public decimal Deposit { get; set; }

        public string Note { get; set; }
        public int OrderItemStatusId { get; set; }
        /// <summary>
        /// Gets the order
        /// </summary>
        public virtual Order Order { get; set; }

        /// <summary>
        /// Gets the product
        /// </summary>
        public virtual Product Product { get; set; }

        public OrderItemStatus OrderItemStatus
        {
            get
            {
                return (OrderItemStatus)OrderItemStatusId;
            }
            set
            {
                OrderItemStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the associated gift card
        /// </summary>
        public virtual ICollection<GiftCard> AssociatedGiftCards
        {
            get { return _associatedGiftCards ?? (_associatedGiftCards = new List<GiftCard>()); }
            protected set { _associatedGiftCards = value; }
        }
    }
}
