using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Shipping
{
    public class Shelf : BaseEntity
    {
        private ICollection<ShelfOrderItem> _shelfOrderItems;
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
        public virtual Customer Customer { get; set; }

        public virtual ICollection<ShelfOrderItem> ShelfOrderItems
        {
            get { return _shelfOrderItems ?? (_shelfOrderItems = new List<ShelfOrderItem>()); }
            protected set { _shelfOrderItems = value; }
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
