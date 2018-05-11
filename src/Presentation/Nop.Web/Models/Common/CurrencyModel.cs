using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Common
{
    public partial class CurrencyModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string CurrencySymbol { get; set; }

        public decimal ExchangeRate { get; set; }

        public bool IsPrimaryExchange { get;set; }
    }
}