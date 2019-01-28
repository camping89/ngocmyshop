using Nop.Core.Domain.Customers;
using System;

namespace Nop.Core.Domain.Shipping
{
    public class Shelf : BaseEntity
    {
        public string ShelfCode { get; set; }

        public int? CustomerId { get; set; }

        public DateTime? AssignedDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
