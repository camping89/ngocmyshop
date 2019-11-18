using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class OrderItemMap : NopEntityTypeConfiguration<OrderItem>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public OrderItemMap()
        {
            this.ToTable("OrderItem");
            this.HasKey(orderItem => orderItem.Id);

            this.Property(orderItem => orderItem.UnitPriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.UnitPriceExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.PriceExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountInclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.DiscountAmountExclTax).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.OriginalProductCost).HasPrecision(18, 4);
            this.Property(orderItem => orderItem.ItemWeight).HasPrecision(18, 4);


            this.HasRequired(orderItem => orderItem.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId);

            this.HasRequired(orderItem => orderItem.Product)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.ProductId);

            this.HasRequired(orderItem => orderItem.PackageOrder)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.PackageOrderId);

            this.HasRequired(orderItem => orderItem.AssignedByCustomer)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.AssignedByCustomerId);

            this.HasRequired(orderItem => orderItem.Shelf)
                .WithMany(s=> s.OrderItems)
                .HasForeignKey(orderItem => orderItem.ShelfId);

            this.HasRequired(orderItem => orderItem.RedeemedRewardPointsOrderItemEntry)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.RewardPointsHistoryEntryId);

            this.Ignore(o => o.OrderItemStatus);
        }
    }
}