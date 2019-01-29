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

        public virtual Customer Customer { get; set; }

        public virtual ICollection<ShelfOrderItem> ShipmentItems
        {
            get { return _shelfOrderItems ?? (_shelfOrderItems = new List<ShelfOrderItem>()); }
            protected set { _shelfOrderItems = value; }
        }
    }
}
