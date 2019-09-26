namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order status enumeration
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Hold
        /// </summary>
        Hold = 5,
        ///// <summary>
        ///// Pending
        ///// </summary>
        //Pending = 10,
        ///// <summary>
        ///// Processing
        ///// </summary>
        //Processing = 20,

        Confirmed = 10,

        PartiallyShipped = 20,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 30,
        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled = 40
    }

    public enum OrderItemStatus
    {
        OutOfStock = 0,
        Available = 1,
        PriceChanged = 2,
        OtherReason = 3
    }
}
