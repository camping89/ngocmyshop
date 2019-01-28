using Nop.Core.Domain.Shipping;

namespace Nop.Data.Mapping.Shipping
{
    public partial class ShelfMap : NopEntityTypeConfiguration<Shelf>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ShelfMap()
        {
            this.ToTable("Shelf");
            this.HasKey(s => s.Id);

            this.HasRequired(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId);
        }
    }
}
