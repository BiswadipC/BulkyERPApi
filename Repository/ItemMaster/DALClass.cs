using Domain.ItemMaster;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ItemMaster
{
    namespace NItemMaster
    {
        internal sealed class DALClass : IItemMasterResponse
        {
            private readonly BulkyContext context;
            private readonly IConfiguration configuration;
            private readonly IMemoryCache cache;

            public DALClass(BulkyContext context, IConfiguration configuration, IMemoryCache cache)
            {
                this.context = context;
                this.configuration = configuration;
                this.cache = cache;
            } // constructor...

            public async Task<List<ItemHeadResponse>> GetItemHeads()
            {
                string key = "ItemHead";

                if(!cache.TryGetValue(key, out List<ItemHeadResponse>? heads))
                {
                    heads = await (from cs in context.ItemHeads
                             select new ItemHeadResponse
                             {
                                 ItemId = cs.ItemId,
                                 ItemName = cs.ItemName,
                                 ReOrderLevel = cs.ReOrderLevel,
                                 PRate = cs.Prate,
                                 SRate = cs.Srate
                             }).ToListAsync();

                    MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    };

                    cache.Set(key, heads, options);
                } // end if...

                return heads!;
            } // GetItemHeads...

            public async Task<ItemHeadResponse> GetItemHeadByItemId(int itemId)
            {
                if(!await context.ItemHeads.AnyAsync(m => m.ItemId == itemId))
                {
                    return new ItemHeadResponse() { ItemId = -1 };
                }

                List<ItemDtlResponse> details = await GetItemDtlsByItemId(itemId);
                ItemGSTResponse itemGST = await GetItemGSTByItemId(itemId);
                ItemOpStockResponse itemOP = await GetItemOpStockByItemId(itemId);

                ItemHeadResponse head = await context.ItemHeads.Select(m => new ItemHeadResponse
                {
                    ItemId = m.ItemId,
                    ItemName = m.ItemName,
                    ReOrderLevel= m.ReOrderLevel,
                    PRate = m.Prate,
                    SRate = m.Srate
                }).FirstOrDefaultAsync(v => v.ItemId == itemId) ?? new ItemHeadResponse();

                head.ListItemDtlResponse = details;
                head.ItemGST = itemGST;
                head.ItemOpStock = itemOP;

                return head;
            } // GetItemHeadByItemId...

            private async Task<List<ItemDtlResponse>> GetItemDtlsByItemId(int itemId)
            {
                List<ItemDtlResponse> details = await (from d in context.ItemDtls
                                                       join a in context.AttrHeads
                                                       on d.AttrHeadIdNo equals a.IdNo
                                                       join b in context.Attrdtls
                                                       on d.AttrDtlIdNo equals b.IdNo
                                                       where d.ItemId == itemId
                                                       select new ItemDtlResponse
                                                       {
                                                           IdNo = d.Idno,
                                                           AttrHeadIdNo = d.AttrHeadIdNo,
                                                           AttrHeadName = a.AttrName,
                                                           AttrDtlIdNo = d.AttrDtlIdNo,
                                                           AttrDtlValue = b.AttrValue
                                                       }).ToListAsync();

                return details ?? new List<ItemDtlResponse>();
            } // GetItemDtlsByItemId...

            public async Task<ItemGSTResponse> GetItemGSTByItemId(int itemId)
            {
                var itemGST = await context.ItemGsts
                            .Where(m => m.ItemId == itemId)
                            .Select(m => new ItemGSTResponse
                            {
                                IdNo = m.IdNo,
                                PurCGSTPerc = m.PurCgstperc,
                                PurSGSTPerc = m.PurSgstperc,
                                PurIGSTPerc = m.PurIgstperc,
                                SalesCGSTPerc = m.SalesCgstperc,
                                SalesSGSTPerc = m.SalesSgstperc,
                                SalesIGSTPerc = m.SalesIgstperc
                            }).FirstOrDefaultAsync();

                return itemGST ?? new ItemGSTResponse();
            } // GetItemGSTByItemId...

            private async Task<ItemOpStockResponse> GetItemOpStockByItemId(int itemId)
            {
                var itemOPStock = await context.ItemOpStocks.Select(m => new ItemOpStockResponse
                {
                    IdNo = m.IdNo,
                    ItemId = m.ItemId,
                    Qty = m.Qty,
                    Rate = m.Rate,
                    Amount = m.Amount
                }).FirstOrDefaultAsync(v => v.ItemId == itemId);

                return itemOPStock ?? new ItemOpStockResponse();
            } // GetItemOpStockByItemId...
            
            public async Task<string> Save(ItemHeadResponse head)
            {
                string message = string.Empty;
                string key = "ItemHead";
                var trans = await context.Database.BeginTransactionAsync();

                try
                {
                    if(head.ItemId == 0)
                    {
                        ItemHead h = new ItemHead();
                        h.ItemId = head.ItemId;
                        h.ItemName = head.ItemName;
                        h.ReOrderLevel = head.ReOrderLevel;
                        h.Prate = head.PRate;
                        h.Srate = head.SRate;
                        await context.AddAsync(h);
                        await context.SaveChangesAsync();

                        if(head.ListItemDtlResponse != null && head.ListItemDtlResponse.Count() > 0)
                        {
                            foreach(var data in head.ListItemDtlResponse)
                            {
                                ItemDtl d = new ItemDtl();
                                d.ItemId = h.ItemId;
                                d.AttrHeadIdNo = data.AttrHeadIdNo ?? 0;
                                d.AttrDtlIdNo = data.AttrDtlIdNo ?? 0;
                                await context.AddAsync(d);
                                await context.SaveChangesAsync();
                            }
                        } // end if...

                        ItemGst gst = new ItemGst();
                        gst.ItemId = h.ItemId;
                        gst.PurCgstperc = head.ItemGST!.PurCGSTPerc ?? 0.00m;
                        gst.PurSgstperc = head.ItemGST.PurSGSTPerc ?? 0.00m;
                        gst.PurIgstperc = head.ItemGST.PurIGSTPerc ?? 0.00m;
                        gst.SalesCgstperc = head.ItemGST.SalesCGSTPerc ?? 0.00m;
                        gst.SalesSgstperc = head.ItemGST.SalesSGSTPerc ?? 0.00m;
                        gst.SalesIgstperc = head.ItemGST.SalesIGSTPerc ?? 0.00m;
                        await context.AddAsync(gst);
                        await context.SaveChangesAsync();

                        if (head.ItemOpStock != null && head.ItemOpStock.Qty > 0)
                        {
                            string systemOpeningDate = configuration.GetValue<string>("SystemOpeningDate") ?? string.Empty;
                            bool b = DateOnly.TryParse(systemOpeningDate, out var d);

                            if(!string.IsNullOrEmpty(systemOpeningDate) && b)
                            {
                                ItemOpStock op = new ItemOpStock();
                                op.ItemId = h.ItemId;
                                op.Qty = head.ItemOpStock!.Qty ?? 0;
                                op.Rate = head.ItemOpStock.Rate ?? 0;
                                op.Amount = head.ItemOpStock.Amount ?? 0;
                                op.SystemOpDate = d;
                                await context.AddAsync(op);
                                await context.SaveChangesAsync();

                                StockDtl stock = new StockDtl();
                                stock.ItemId = h.ItemId;
                                stock.ModuleName = "STOCKDTL";
                                stock.DocId = h.ItemId;
                                stock.DocNo = h.ItemId.ToString();
                                stock.DocDate = d;
                                stock.DtlRecId = h.ItemId;
                                stock.InQty = head.ItemOpStock.Qty ?? 0;
                                stock.OutQty = 0;
                                stock.Rate = head.ItemOpStock.Rate ?? 0;
                                stock.Mrp = head.SRate;
                                stock.Amount = (stock.InQty * stock.Rate);

                                await context.AddAsync(stock);
                                await context.SaveChangesAsync();
                            } // end if...
                        } // stock entry...
                    } // new...
                    else
                    {
                        var existingItemHead = await context.ItemHeads.FirstOrDefaultAsync(m => m.ItemId == head.ItemId);
                        existingItemHead!.ItemName = head.ItemName;
                        existingItemHead.ReOrderLevel = head.ReOrderLevel;
                        existingItemHead.Prate = head.PRate;
                        existingItemHead.Srate = head.SRate;
                        context.Update(existingItemHead);
                        await context.SaveChangesAsync();

                        if (head.ListItemDtlResponse != null && head.ListItemDtlResponse.Count() > 0)
                        {                   
                            List<int> incomingIds = head.ListItemDtlResponse.Select(m => m.IdNo).ToList();
                            var existingDtlRecords = context.ItemDtls.Where(m => m.ItemId == head.ItemId).ToList();
                            List<ItemDtl> recordsToBeDeleted = existingDtlRecords.Where(m => !incomingIds.Contains(m.Idno)).ToList();
                                
                            context.ItemDtls.RemoveRange(recordsToBeDeleted);
                            await context.SaveChangesAsync();

                            foreach (var data in head.ListItemDtlResponse)
                            {
                                if (data.IdNo == 0)
                                {
                                    ItemDtl d = new ItemDtl();
                                    d.ItemId = head.ItemId;
                                    d.AttrHeadIdNo = data.AttrHeadIdNo ?? 0;
                                    d.AttrDtlIdNo = data.AttrDtlIdNo ?? 0;
                                    await context.AddAsync(d);
                                    await context.SaveChangesAsync();
                                } // new record...
                                else
                                {
                                    var existingDtl = await context.ItemDtls.FirstOrDefaultAsync(m => m.Idno == data.IdNo);
                                    existingDtl!.AttrHeadIdNo = (int)data.AttrHeadIdNo!;
                                    existingDtl.AttrDtlIdNo = (int)data.AttrDtlIdNo!;
                                    context.Update(existingDtl);
                                    await context.SaveChangesAsync();
                                } // existing record...
                            }
                        } // end if...

                        var existingItemGST = await context.ItemGsts.FirstOrDefaultAsync(m => m.ItemId == head.ItemId);
                        existingItemGST!.PurCgstperc = head.ItemGST!.PurCGSTPerc ?? 0.00m;
                        existingItemGST.PurSgstperc = head.ItemGST.PurSGSTPerc ?? 0.00m;
                        existingItemGST.PurIgstperc = head.ItemGST.PurIGSTPerc ?? 0.00m;
                        existingItemGST.SalesCgstperc = head.ItemGST.SalesCGSTPerc ?? 0.00m;
                        existingItemGST.SalesSgstperc = head.ItemGST.SalesSGSTPerc ?? 0.00m;
                        existingItemGST.SalesIgstperc = head.ItemGST.SalesIGSTPerc ?? 0.00m;
                        context.Update(existingItemGST);
                        await context.SaveChangesAsync();

                        var existingItemOp = await context.ItemOpStocks.Where(m => m.ItemId == head.ItemId).FirstOrDefaultAsync();
                        if(existingItemOp != null)
                        {
                            context.ItemOpStocks.Remove(existingItemOp);
                            await context.SaveChangesAsync();
                        } // end if...

                        var existingStocks = await context.StockDtls.Where(m => m.ItemId == head.ItemId && m.ModuleName == "STOCKDTL").ToListAsync();
                        if(existingStocks != null)
                        {
                            context.StockDtls.RemoveRange(existingStocks);
                            await context.SaveChangesAsync();
                        } // end if...

                        if (head.ItemOpStock != null && head.ItemOpStock.Qty > 0)
                        {
                            string systemOpeningDate = configuration.GetValue<string>("SystemOpeningDate") ?? string.Empty;
                            bool b = DateOnly.TryParse(systemOpeningDate, out var d);

                            if (!string.IsNullOrEmpty(systemOpeningDate) && b)
                            {
                                ItemOpStock op = new ItemOpStock();
                                op.ItemId = head.ItemId;
                                op.Qty = head.ItemOpStock!.Qty ?? 0;
                                op.Rate = head.ItemOpStock.Rate ?? 0;
                                op.Amount = head.ItemOpStock.Amount ?? 0;
                                op.SystemOpDate = d;
                                await context.AddAsync(op);
                                await context.SaveChangesAsync();

                                StockDtl stock = new StockDtl();
                                stock.ItemId = head.ItemId;
                                stock.ModuleName = "STOCKDTL";
                                stock.DocId = head.ItemId;
                                stock.DocNo = head.ItemId.ToString();
                                stock.DocDate = d;
                                stock.DtlRecId = head.ItemId;
                                stock.InQty = head.ItemOpStock.Qty ?? 0;
                                stock.OutQty = 0;
                                stock.Rate = head.ItemOpStock.Rate ?? 0;
                                stock.Mrp = head.SRate;
                                stock.Amount = (stock.InQty * stock.Rate);

                                await context.AddAsync(stock);
                                await context.SaveChangesAsync();
                            } // end if...
                        } // stock entry...
                    } // edit...

                    await trans.CommitAsync();
                    cache.Remove(key);
                    message = "Success";
                } // end of try...
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    message = ex.ToString();
                } // end of catch...
                finally
                {
                    trans.Dispose();
                } // end of finally...

                return message;
            } // Save...
        } // class...
    } // namespace NItemMaster...
}
