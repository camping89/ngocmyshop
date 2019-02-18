using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class DistrictMap : NopEntityTypeConfiguration<District>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public DistrictMap()
        {
            this.ToTable("District");
            this.HasKey(sp => sp.Id);
            this.Property(sp => sp.Name).IsRequired().HasMaxLength(150);

            this.HasRequired(sp => sp.StateProvince)
                .WithMany(c => c.Districts)
                .HasForeignKey(sp => sp.StateProvinceId);
        }
    }
}