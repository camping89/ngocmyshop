using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    public partial class ShelfOrderItemMap : NopEntityTypeConfiguration<ShelfOrderItem>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ShelfOrderItemMap()
        {
            this.ToTable("ShelfOrderItem");
            this.HasKey(s => s.Id);

            this.HasRequired(s => s.OrderItem)
                .WithMany()
                .HasForeignKey(s => s.OrderItemId);

            this.HasRequired(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId);

            this.HasRequired(s => s.Shelf)
                .WithMany()
                .HasForeignKey(s => s.ShelfId);
        }
    }
}
