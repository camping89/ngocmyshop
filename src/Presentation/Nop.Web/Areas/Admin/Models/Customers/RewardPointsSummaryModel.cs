using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public class RewardPointsSummaryModel  : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Customer.RewardHistorySummary.FromDateTime")]
        [UIHint("DateNullable")]
        public DateTime? FromDateTime { get; set; }

        [NopResourceDisplayName("Admin.Customer.RewardHistorySummary.ToDateTime")]
        [UIHint("DateNullable")]
        public DateTime? ToDateTime { get; set; }
    }
}
