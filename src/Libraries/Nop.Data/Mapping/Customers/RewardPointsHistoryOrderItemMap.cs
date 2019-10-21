using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class RewardPointsHistoryOrderItemMap : NopEntityTypeConfiguration<RewardPointsHistoryOrderItem>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public RewardPointsHistoryOrderItemMap()
        {
            this.ToTable("RewardPointsHistory_OrderItem");
            this.HasKey(rph => rph.Id);

            this.Property(rph => rph.UsedAmount).HasPrecision(18, 4);

            this.HasRequired(rph => rph.Customer)
                .WithMany()
                .HasForeignKey(rph => rph.CustomerId);

            this.HasOptional(rph => rph.UsedWithOrderItem)
                .WithOptionalDependent(o => o.RedeemedRewardPointsOrderItemEntry)
                .WillCascadeOnDelete(false);
        }
    }
}