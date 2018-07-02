using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Orders
{
    public class ExportVendorInvoiceModel{
        public string CustomerInfo { get;set; }
        public string ProductName { get;set; }
        public string Sku { get;set; }
        public string PriceBase { get;set; }
        public double DiscountPercent { get; set; }
        public string Price { get;set; }
        public string Total { get;set; }
        public string VendorProductUrl { get;set; }
        public string AdminNote { get; set; }

    }
}
