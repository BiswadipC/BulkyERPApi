using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Domain.PurchaseOrder;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.PurchaseOrder
{
    namespace NPurchaseOrder
    {
        internal sealed class DALClass: IPurchaseOrder
        {
            private readonly BulkyContext context;
            private readonly IDbConnection idb;

            public DALClass(BulkyContext context, IDbConnection idb)
            {
                this.context = context;
                this.idb = idb;
            } // constructor...

            public async Task<List<PurchaseOrderHeadResponse>> GetPOHeads()
            {
                return await (from h in context.PurOrderHeads
                             join p in context.PartyMasters
                             on h.PartyCode equals p.PartyCode
                             select new PurchaseOrderHeadResponse
                             {
                                 OrderId = h.OrderId,
                                 OrderNo = h.OrderNo,
                                 OrderDate = h.OrderDate.ToString("dd/MM/yyyy"),
                                 PartyCode = h.PartyCode,
                                 PartyName = p.PartyName,
                                 TotalAmount = h.TotalAmount,
                                 Remarks = h.Remarks
                             }).ToListAsync();
            } // GetPOHeads...

            public async Task<List<PurchaseOrderDtlResponse>> GetPODtlsByOrderId(int? orderId)
            {
                var dtls = await (from d in context.PurOrderDtls
                                  join i in context.ItemHeads
                                  on d.ItemId equals i.ItemId
                                  where d.OrderId == orderId
                                  select new PurchaseOrderDtlResponse
                                  {
                                      IdNo = d.IdNo,
                                      ItemId = d.ItemId,
                                      ItemName = i.ItemName,
                                      Qty = d.Qty,
                                      Rate = d.Rate,
                                      Amount = d.Amount
                                  }).ToListAsync();

                return dtls;
            } // GetPODtlsByOrderId...

            public async Task<PurchaseOrderHeadResponse> GetPOHeadByOrderId(int? orderId)
            {
                var head = await (from h in context.PurOrderHeads
                                  join p in context.PartyMasters
                                  on h.PartyCode equals p.PartyCode
                                  select new PurchaseOrderHeadResponse
                                  {
                                      OrderId = h.OrderId,
                                      OrderNo = h.OrderNo,
                                      OrderDate = h.OrderDate.ToString("dd/MM/yyyy"),
                                      PartyCode = h.PartyCode,
                                      PartyName = p.PartyName,
                                      TotalAmount = h.TotalAmount,
                                      Remarks = h.Remarks
                                  }).FirstOrDefaultAsync(m => m.OrderId == orderId);
                head!.ListPoDtls = await GetPODtlsByOrderId(orderId);

                return head;
            } // GetPOHeadByOrderId...

            public async Task<List<PurchaseOrderHeadResponse>> GetPOHeadsByPartyCode(int? partyCode)
            {
                string str = @"select h.OrderId, h.OrderNo, Convert(varchar, h.OrderDate, 103) OrderDate, h.PartyCode, p.PartyName,
		                                h.TotalAmount, h.Remarks from PurOrderHead h inner join PartyMaster p on (h.PartyCode = p.PartyCode)
                                where h.PartyCode = @pcode
                                and OrderId in (select OrderId from PurOrderDtl
				                                where isnull(Qty,0) - isnull(PbQtyAdj,0) > 0
	                                group by OrderId
	                                having count(1) >= 0
                                )";
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@pcode", partyCode);
                List<PurchaseOrderHeadResponse> heads = (await idb.QueryAsync<PurchaseOrderHeadResponse>(str, dp)).ToList();
                return heads;
            } // GetPOHeadsByPartyCode...

            public async Task<PurchaseOrderDtlResponse> GetPODtlByPODtlRecId(int? recId)
            {
                var dtl = await (from d in context.PurOrderDtls
                                  join i in context.ItemHeads
                                  on d.ItemId equals i.ItemId
                                  where d.IdNo == recId
                                  select new PurchaseOrderDtlResponse
                                  {
                                      IdNo = d.IdNo,
                                      ItemId = d.ItemId,
                                      ItemName = i.ItemName,
                                      Qty = d.Qty,
                                      Rate = d.Rate,
                                      Amount = d.Amount
                                  }).FirstOrDefaultAsync();

                return dtl ?? new PurchaseOrderDtlResponse();
            } // GetPODtlByPODtlRecId...

            public async Task<string> Save(PurchaseOrderHeadResponse response)
            {
                string message = string.Empty;
                var trans = await context.Database.BeginTransactionAsync();

                try
                {
                    if(context.PurOrderHeads.Any(m => m.OrderNo.ToLower() == response.OrderNo!.ToLower() && response.OrderId == 0))
                    {
                        message = $"Order No. {response.OrderNo} already exists.";
                        return message;
                    }

                    if(context.PurOrderHeads.Any(m => m.OrderNo.ToLower() == response.OrderNo!.ToLower() && m.OrderId != response.OrderId))
                    {
                        message = $"Order No. {response.OrderNo} already exists.";
                        return message;
                    }

                    if(response.OrderId == 0)
                    {
                        PurOrderHead head = new PurOrderHead();
                        head.OrderId = response.OrderId;
                        head.OrderNo = response.OrderNo!;
                        head.OrderDate = DateOnly.ParseExact(response.OrderDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        head.PartyCode = (int)response.PartyCode!;
                        head.TotalAmount = (decimal)response.ListPoDtls!.Sum(x => x.Amount)!;
                        head.Remarks = response.Remarks;
                        head.ApprovalStatus = "Pending";                        
                        await context.AddAsync(head);
                        await context.SaveChangesAsync();

                        foreach(var data in response.ListPoDtls!)
                        {
                            PurOrderDtl dtl = new PurOrderDtl();
                            dtl.OrderId = head.OrderId;
                            dtl.OrderNo = head.OrderNo!;
                            dtl.OrderDate = head.OrderDate;
                            dtl.ItemId = (int)data.ItemId!;
                            dtl.Qty = (int)data.Qty!;
                            dtl.Rate = (decimal)data.Rate!;
                            dtl.Amount = (decimal)data.Amount!;
                            await context.AddAsync(dtl);
                            await context.SaveChangesAsync();
                        }                        
                    } // new...
                    else
                    {
                        var existingHead = await context.PurOrderHeads.FirstOrDefaultAsync(x => x.OrderId == response.OrderId);
                        existingHead!.OrderNo = response.OrderNo!;
                        existingHead.OrderDate = DateOnly.ParseExact(response.OrderDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        existingHead.PartyCode = (int)response.PartyCode!;
                        existingHead.TotalAmount = (decimal)response.ListPoDtls!.Sum(x => x.Amount)!;
                        existingHead.Remarks = response.Remarks;
                        context.Update(existingHead);
                        await context.SaveChangesAsync();

                        /********************************************* delete ************************************************************/
                        var existingDatabaseRecords = await context.PurOrderDtls.Where(x => x.OrderId == response.OrderId).ToListAsync();
                        List<int> incomingIds = response.ListPoDtls!.Where(m => m.IdNo != 0).Select(x => x.IdNo).ToList();
                        List<PurOrderDtl> recordsTobeDeleted = existingDatabaseRecords.Where(x => !incomingIds.Contains(x.IdNo)).ToList();
                        context.RemoveRange(recordsTobeDeleted);
                        await context.SaveChangesAsync();
                        /*****************************************************************************************************************/
                        foreach(var data in response.ListPoDtls!)
                        {
                            if(data.IdNo == 0)
                            {
                                PurOrderDtl dtl = new PurOrderDtl();
                                dtl.OrderId = response.OrderId;
                                dtl.OrderNo = response.OrderNo!;
                                dtl.OrderDate = DateOnly.ParseExact(response.OrderDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                dtl.ItemId = (int)data.ItemId!;
                                dtl.Qty = (int)data.Qty!;
                                dtl.Rate = (decimal)data.Rate!;
                                dtl.Amount = (decimal)data.Amount!;
                                await context.AddAsync(dtl);
                                await context.SaveChangesAsync();
                            }
                            else
                            {
                                var existingDtl = await context.PurOrderDtls.FirstOrDefaultAsync(m => m.IdNo == data.IdNo);
                                existingDtl!.OrderNo = response.OrderNo!;
                                existingDtl.OrderDate = DateOnly.ParseExact(response.OrderDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                existingDtl.ItemId = (int)data.ItemId!;
                                existingDtl.Qty = (int)data.Qty!;
                                existingDtl.Rate = (decimal)data.Rate!;
                                existingDtl.Amount = (decimal)data.Amount!;
                                context.Update(existingDtl);
                                await context.SaveChangesAsync();
                            }
                        }
                    } // edit, end if...

                    await trans.CommitAsync();
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
    } // NPurchaseOrder...
}