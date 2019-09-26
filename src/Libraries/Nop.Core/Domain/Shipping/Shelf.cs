using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Shipping
{
    public class Shelf : BaseEntity
    {
        private ICollection<OrderItem> _orderItems;
        public string ShelfCode { get; set; }

        public int? CustomerId { get; set; }

        public DateTime? AssignedDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? UpdatedNoteDate { get; set; }
        public bool IsCustomerNotified { get; set; }
        public int ShelfNoteId { get; set; }

        public decimal Total { get; set; }
        public decimal TotalWithoutDeposit { get; set; }
        public bool HasOrderItem { get; set; }
        public bool InActive { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems
        {
            get { return _orderItems ?? (_orderItems = new List<OrderItem>()); }
            protected set { _orderItems = value; }
        }

        /// <summary>
        /// Gets or sets the shipping status
        /// </summary>
        public ShelfNoteStatus ShelfNoteStatus
        {
            get
            {
                return (ShelfNoteStatus)ShelfNoteId;
            }
            set
            {
                ShelfNoteId = (int)value;
            }
        }
    }
}
