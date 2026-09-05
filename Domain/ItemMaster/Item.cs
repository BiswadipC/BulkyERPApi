using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ItemMaster
{
    public class ItemHeadResponse
    {
        public int ItemId {  get; set; }
        public string ItemName { get; set; } = string.Empty;
        public  decimal? ReOrderLevel {  get; set; }
        public decimal? PRate {  get; set; }
        public decimal? SRate { get; set; }
        public List<ItemDtlResponse>? ListItemDtlResponse {  get; set; } = new List<ItemDtlResponse>();
        public ItemGSTResponse? ItemGST {  get; set; } = new ItemGSTResponse();
        public ItemOpStockResponse? ItemOpStock { get; set; } = new ItemOpStockResponse();
    } // class...

    public class ItemDtlResponse
    {
        public int IdNo {  get; set; }
        public int? AttrHeadIdNo { get; set; }
        public string? AttrHeadName { get; set; } = string.Empty;
        public int? AttrDtlIdNo {  get; set; }
        public string? AttrDtlValue { get; set; } = string.Empty;
    } // class...

    public class ItemGSTResponse
    {
        public int IdNo { get; set; }
        public decimal? PurCGSTPerc {  get; set; }
        public decimal? PurSGSTPerc { get; set; }
        public decimal? PurIGSTPerc { get; set; }
        public decimal? SalesCGSTPerc { get; set; }
        public decimal? SalesSGSTPerc { get; set; }
        public decimal? SalesIGSTPerc { get; set; }
    } // class...

    public class ItemOpStockResponse
    {
        public int IdNo { get; set; }
        public int? ItemId { get; set; }
        public int? Qty {  get; set; }
        public decimal? Rate {  get; set; }
        public decimal? Amount {  get; set; }
    } // class...
}
