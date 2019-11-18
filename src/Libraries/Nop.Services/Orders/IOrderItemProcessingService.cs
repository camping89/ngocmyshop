using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    public interface IOrderItemProcessingService
    {
        void AwardRewardPoints(OrderItem orderItem);
        void ReduceRewardPoints(OrderItem orderItem);
        void ReturnBackRedeemedRewardPoints(OrderItem orderItem);
    }
}
