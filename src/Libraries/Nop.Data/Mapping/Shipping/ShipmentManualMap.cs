using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ShipmentManualMap : NopEntityTypeConfiguration<ShipmentManual>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ShipmentManualMap()
        {
            this.ToTable("ShipmentManual");
            this.HasKey(s => s.Id);

            this.Property(s => s.TotalWeight).HasPrecision(18, 4);

            this.HasRequired(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId);

            this.HasRequired(s => s.Shipper)
                .WithMany()
                .HasForeignKey(s => s.ShipperId);

            this.HasRequired(s => s.ShippingAddress)
                .WithMany()
                .HasForeignKey(s => s.ShippingAddressId);
        }
    }
}