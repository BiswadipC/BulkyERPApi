using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Reports.Stock
{
    public class StockVsReOrderClass
    {
        public int ItemId {  get; set; }
        public string ItemName {  get; set; } = string.Empty;
        public int StockQty {  get; set; }
        public int ReOrderLevel { get; set; }
        public int Diff {  get; set; }
        public decimal MRP {  get; set; }
    } // class...
}
