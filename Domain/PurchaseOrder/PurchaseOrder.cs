using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.PurchaseOrder
{
    public class PurchaseOrderHeadResponse
    {
        public int OrderId {  get; set; }
        public string? OrderNo { get; set; } = string.Empty;
        public string? OrderDate {  get; set; }
        public int? PartyCode {  get; set; }
        public string? PartyName { get; set; } = string.Empty;
        public decimal? TotalAmount { get; set; } = 0.00m;
        public string? Remarks { get; set; } = string.Empty;
        public List<PurchaseOrderDtlResponse>? ListPoDtls {  get; set; } = new List<PurchaseOrderDtlResponse>();
    } // class...

    public class PurchaseOrderDtlResponse
    {
        public int IdNo {  get; set; }
        public int? ItemId {  get; set; }
        public string? ItemName { get; set; } = string.Empty;
        public int? Qty {  get; set; }
        public decimal? Rate { get; set; } = 0.00m;
        public decimal? Amount { set; get; } = 0.00m;
    } // class...
}
