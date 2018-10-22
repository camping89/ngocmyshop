using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class PackageOrderMap : NopEntityTypeConfiguration<PackageOrder>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public PackageOrderMap()
        {
            this.ToTable("PackageOrder");
            this.HasKey(gc => gc.Id);
        }
    }
}