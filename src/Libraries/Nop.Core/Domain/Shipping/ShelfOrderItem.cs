using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System;

namespace Nop.Core.Domain.Shipping
{
    public class ShelfOrderItem : BaseEntity
    {
        public ShelfOrderItem()
        {
            IsActived = true;
        }
        public int ShelfId { get; set; }
        public int OrderItemId { get; set; }
        public int CustomerId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActived { get; set; }
        public virtual Shelf Shelf { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual OrderItem OrderItem { get; set; }
    }
}
