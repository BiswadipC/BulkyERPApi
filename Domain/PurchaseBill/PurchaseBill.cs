using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.PurchaseBill
{
    public class PurchaseBillHeadResponse
    {
        public int BillId { get; set; }
        public string? BillNo { get; set; } = string.Empty;
        public string? BillDate { get; set; }
        public int? LedgerId {  get; set; }
        public string? LedgerName { get; set; } = string.Empty;
        public int? PartyCode { get; set; }
        public string? PartyName { get; set; } = string.Empty;
        public decimal? NetAmount { get; set; } = 0.00m;
        public string? Remarks { get; set; } = string.Empty;
        public List<PurchaseBillDtlResponse>? ListPBDtls {  get; set; } = new List<PurchaseBillDtlResponse>();
    } // class...

    public class PurchaseBillDtlResponse
    {
        public int IdNo { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; } = string.Empty;
        public int OrderQty { get; set; }
        public int AdjustedQty {  get; set; }
        public int PurchaseQty {  get; set; }
        public int BalanceQty {  get; set; }
        public decimal Rate { get; set; } = 0.00m;
        public decimal Amount { set; get; } = 0.00m;
        public decimal? CGST {  get; set; }
        public decimal? SGST { get; set; }
        public decimal? IGST { get; set; }
        public decimal? DiscountPerc { set; get; } = 0.00m;
        public decimal? DiscountValue { set; get; } = 0.00m;
        public decimal? TotalAmount { set; get; } = 0.00m;
        public decimal? AmountAfterDiscount { set; get; } = 0.00m;
        public int? PODtlIdNo {  get; set; }
        public int? OrderId { get; set; }
        public string? OrderNo { get; set; } = string.Empty;
    } // class...
}
