using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Localization;
using System;

namespace Nop.Services.Orders
{
    public class OrderItemProcessingService : IOrderItemProcessingService
   {
       private readonly IOrderTotalCalculationService _orderTotalCalculationService;
       private readonly RewardPointsSettings _rewardPointsSettings;
       private readonly IRewardPointOrderItemService _rewardPointOrderItemService;
       private readonly IOrderService _orderService;
       private readonly ILocalizationService _localizationService;
       public OrderItemProcessingService(IOrderTotalCalculationService orderTotalCalculationService, RewardPointsSettings rewardPointsSettings, IRewardPointOrderItemService rewardPointOrderItemService, IOrderService orderService, ILocalizationService localizationService)
       {
           _orderTotalCalculationService = orderTotalCalculationService;
           _rewardPointsSettings = rewardPointsSettings;
           _rewardPointOrderItemService = rewardPointOrderItemService;
           _orderService = orderService;
           _localizationService = localizationService;
       }

       public void AwardRewardPoints(OrderItem orderItem)
        {
            var totalForRewardPoints = orderItem.PriceExclTax;
            var points = _orderTotalCalculationService.CalculateRewardPoints(orderItem.Order.Customer, totalForRewardPoints);
            if (points == 0)
                return;

            //Ensure that reward points were not added (earned) before. We should not add reward points if they were already earned for this order
            if (orderItem.RewardPointsHistoryEntryId.HasValue)
                return;

            //check whether delay is set
            DateTime? activatingDate = null;
            if (_rewardPointsSettings.ActivationDelay > 0)
            {
                var delayPeriod = (RewardPointsActivatingDelayPeriod)_rewardPointsSettings.ActivationDelayPeriodId;
                var delayInHours = delayPeriod.ToHours(_rewardPointsSettings.ActivationDelay);
                activatingDate = DateTime.UtcNow.AddHours(delayInHours);
            }

            //add reward points
            orderItem.RewardPointsHistoryEntryId = _rewardPointOrderItemService.AddRewardPointsHistoryEntry(orderItem.Order.Customer, points, orderItem.Order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.EarnedForOrderItem"), orderItem.Id), activatingDate: activatingDate);

            _orderService.UpdateOrderItem(orderItem);
        }

        public void ReduceRewardPoints(OrderItem orderItem)
        {
            var totalForRewardPoints = orderItem.PriceInclTax;
            var points = _orderTotalCalculationService.CalculateRewardPoints(orderItem.Order.Customer, totalForRewardPoints);
            if (points == 0)
                return;

            //ensure that reward points were already earned for this order before
            if (!orderItem.RewardPointsHistoryEntryId.HasValue)
                return;

            //get appropriate history entry
            var rewardPointsHistoryEntry = _rewardPointOrderItemService.GetRewardPointsHistoryEntryById(orderItem.RewardPointsHistoryEntryId.Value);
            if (rewardPointsHistoryEntry != null && rewardPointsHistoryEntry.CreatedOnUtc > DateTime.UtcNow)
            {
                //just delete the upcoming entry (points were not granted yet)
                _rewardPointOrderItemService.DeleteRewardPointsHistoryEntry(rewardPointsHistoryEntry);
            }
            else
            {
                //or reduce reward points if the entry already exists
                _rewardPointOrderItemService.AddRewardPointsHistoryEntry(orderItem.Order.Customer, -points, orderItem.Order.StoreId,
                    string.Format(_localizationService.GetResource("RewardPoints.Message.ReducedForOrderItem"), orderItem.Id));
            }

            _orderService.UpdateOrderItem(orderItem);
        }

        public void ReturnBackRedeemedRewardPoints(OrderItem orderItem)
        {
            //were some points redeemed when placing an order?
            if (orderItem.RedeemedRewardPointsOrderItemEntry == null)
                return;

            //return back
            _rewardPointOrderItemService.AddRewardPointsHistoryEntry(orderItem.Order.Customer, -orderItem.RedeemedRewardPointsOrderItemEntry.Points, orderItem.Order.StoreId,
                string.Format(_localizationService.GetResource("RewardPoints.Message.ReturnedForOrderItem"), orderItem.Id));
            _orderService.UpdateOrderItem(orderItem);
        }
    }
}
