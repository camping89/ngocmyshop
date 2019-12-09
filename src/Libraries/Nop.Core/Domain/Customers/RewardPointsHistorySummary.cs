using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Customers
{
    public class RewardPointsHistorySummary
    {
        public int CustomerId { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerFullName { get; set; }
        public int TotalPoint { get; set; }
    }
}
