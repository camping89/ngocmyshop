using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ShipmentManualItemMap : NopEntityTypeConfiguration<ShipmentManualItem>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ShipmentManualItemMap()
        {
            this.ToTable("ShipmentManualItem");
            this.HasKey(si => si.Id);

            this.HasRequired(si => si.ShipmentManual)
                .WithMany(s => s.ShipmentManualItems)
                .HasForeignKey(si => si.ShipmentManualId);
        }
    }
}