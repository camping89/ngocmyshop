using System;

namespace Nop.Core.Domain.Orders
{
    public class PackageOrder : BaseEntity
    {
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public DateTime? ArrivalDatetime { get; set; }
        public DateTime? PackageProcessedDatetime { get; set; }
        public bool IsShipped { get; set; }
    }
}
