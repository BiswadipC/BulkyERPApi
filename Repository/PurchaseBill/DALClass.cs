using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.PurchaseBill;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.PurchaseBill
{
    namespace NPurchaseBill
    {
        internal class DALClass : IPurchaseBill
        {
            private readonly BulkyContext context;
            private readonly IDbConnection db;

            public DALClass(BulkyContext context, IDbConnection db)
            {
                this.context = context;
                this.db = db;
            } // constructor...

            public async Task<List<PurchaseBillHeadResponse>> GetPurchaseBills()
            {
                return await
                (from h in context.PurBillHeads
                 join l in context.Accounts
                 on h.AccountId equals l.AccountId
                 join p in context.PartyMasters
                 on h.PartyCode equals p.PartyCode into G1
                 from p in G1.DefaultIfEmpty()
                 select new PurchaseBillHeadResponse
                 {
                     BillId = h.BillId,
                     BillNo = h.BillNo,
                     BillDate = h.BillDate.ToString("dd/MM/yyyy"),
                     LedgerId = h.AccountId,
                     LedgerName = l.AccountName,
                     PartyCode = h.PartyCode,
                     PartyName = p.PartyName,
                     NetAmount = h.NetAmount,
                     Remarks = h.Remarks
                 }).ToListAsync();
            } // GetPurchaseBills...

            public async Task<List<PurchaseBillDtlResponse>> GetPurchaseBillDtlsByBillId(int billId)
            {
                string str = @"select d.IdNo, d.ItemId, i.ItemName, od.Qty OrderQty, isnull(od.PbQtyAdj,0) AdjustedQty, d.Qty PurchaseQty,
                                      (isnull(od.Qty,0) - isnull(od.PbQtyAdj,0)) BalanceQty, d.Rate, d.Amount, 
                                      d.CGST, d.SGST, d.IGST, d.DiscountPerc, d.DiscountValue, d.TotalAmount, d.AmountAfterDiscount, 
                                      d.PODtlIdNo, d.OrderId, od.OrderNo
                                  from PurBillDtl d inner join ItemHead i on (i.ItemId = d.ItemId)
                                  left join PurOrderDtl od on (d.PODtlIdNo = od.IdNo)
                                 where d.BillId = @id";
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@id", billId);

                List<PurchaseBillDtlResponse> dtls = (await db.QueryAsync<PurchaseBillDtlResponse>(str, dp)).ToList();
                return dtls;
            } // GetPurchaseBillDtlsByBillId...

            public async Task<PurchaseBillHeadResponse> GetPurchaseBillHeadByBillId(int billId)
            {
                PurchaseBillHeadResponse head = new PurchaseBillHeadResponse();
                head = await (from h in context.PurBillHeads
                        join l in context.Accounts
                        on h.AccountId equals l.AccountId
                        join p in context.PartyMasters
                        on h.PartyCode equals p.PartyCode into G1
                        from p in G1.DefaultIfEmpty()
                        select new PurchaseBillHeadResponse
                        {
                            BillId = h.BillId,
                            BillNo = h.BillNo,
                            BillDate = h.BillDate.ToString("dd/MM/yyyy"),
                            LedgerId = h.AccountId,
                            LedgerName = l.AccountName,
                            PartyCode = h.PartyCode,
                            PartyName = p.PartyName,
                            NetAmount = h.NetAmount,
                            Remarks = h.Remarks
                        }).FirstOrDefaultAsync(m => m.BillId == billId) ?? new PurchaseBillHeadResponse();

                head.ListPBDtls = await GetPurchaseBillDtlsByBillId(billId);
                return head;
            } // GetPurchaseBillHeadByBillId...

            public async Task<string> SaveBill(PurchaseBillHeadResponse response)
            {
                string message = string.Empty;
                int docId = 0;
                var trans = await context.Database.BeginTransactionAsync();

                try
                {
                    if(response.BillId == 0)
                    {
                        if(context.PurBillHeads.Any(m => m.BillNo.ToUpper() == response.BillNo!.ToUpper()))
                        {
                            message = $"Bill No. {response.BillNo} already exists.";
                            return message;
                        }
                    }

                    if(response.BillId > 0)
                    {
                        if(context.PurBillHeads.Any(m => m.BillNo.ToUpper() == response.BillNo!.ToUpper() && m.BillId != response.BillId))
                        {
                            message = $"Bill No. {response.BillNo} already exists.";
                            return message;
                        }
                    }

                    string strCheckCABA = "select count(1) from Accounts where AccountId = @id and Category in ('CA', 'BA')";
                    DynamicParameters dp = new DynamicParameters();
                    dp.Add("@id", response.LedgerId);
                    int result = Convert.ToInt32((await db.QuerySingleAsync<string>(strCheckCABA, dp)));
                    bool b = result > 0 ? true : false;

                    int cgstAccountId = Convert.ToInt32(await (from cs in context.SystemVariables
                                                where cs.VariableName == "Input CGST"
                                                select cs.VariableValue).FirstOrDefaultAsync());
                    
                    int sgstAccountId = Convert.ToInt32(await (from cs in context.SystemVariables
                                                               where cs.VariableName == "Input SGST"
                                                               select cs.VariableValue).FirstOrDefaultAsync());
                    
                    int igstAccountId = Convert.ToInt32(await (from cs in context.SystemVariables
                                                               where cs.VariableName == "Input IGST"
                                                               select cs.VariableValue).FirstOrDefaultAsync());

                    int discountAccountId = Convert.ToInt32(await (from cs in context.SystemVariables
                                                               where cs.VariableName == "Discount Received"
                                                                   select cs.VariableValue).FirstOrDefaultAsync());

                    int purCode = Convert.ToInt32((await (from cs in context.SystemVariables
                                                          where cs.VariableName == "Purchase Account"
                                                          select new { cs.VariableValue }).FirstOrDefaultAsync())!.VariableValue);

                    if (response.BillId == 0)
                    {
                        PurBillHead h = new PurBillHead();
                        h.BillId = response.BillId;
                        h.BillNo = response.BillNo!;
                        h.BillDate = DateOnly.ParseExact(response.BillDate!,"dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        h.PartyCode = response.PartyCode;
                        h.AccountId = (int)response.LedgerId!;
                        h.PurAccountId = purCode;
                        h.NetAmount = (decimal)response.ListPBDtls!.Sum(m => m.AmountAfterDiscount)!;
                        h.AccountsAdjAmount = b ? h.NetAmount : 0.00m;
                        h.Remarks = response.Remarks ?? string.Empty;
                        await context.AddAsync(h);
                        await context.SaveChangesAsync();

                        docId = h.BillId;

                        foreach(var data in response.ListPBDtls!)
                        {
                            PurBillDtl d = new PurBillDtl();
                            d.BillId = h.BillId;
                            d.BillNo = h.BillNo;
                            d.BillDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            d.ItemId = data.ItemId;
                            d.Qty = data.PurchaseQty;
                            d.Rate = data.Rate;
                            d.Amount = data.Amount;
                            d.Cgst = data.CGST;
                            d.Sgst = data.SGST;
                            d.Igst = (data.CGST + data.SGST);
                            d.CgstledgerId = cgstAccountId;
                            d.SgstledgerId = sgstAccountId;
                            d.IgstledgerId = igstAccountId;
                            d.DiscountPerc = data.DiscountPerc;
                            d.DiscountValue = data.DiscountValue;
                            d.DiscountLedgerId = data.DiscountValue == 0 ? null : discountAccountId;
                            d.TotalAmount = (data.Amount + data.CGST + data.SGST);
                            d.AmountAfterDiscount = data.AmountAfterDiscount;
                            d.PodtlIdNo = data.PODtlIdNo == 0 ? null : data.PODtlIdNo;
                            d.OrderId = data.OrderId == 0 ? null : data.OrderId;
                            await context.AddAsync(d);
                            await context.SaveChangesAsync();

                            StockDtl dtl = new StockDtl();
                            dtl.ItemId = data.ItemId;
                            dtl.ModuleName = "PURCHASE BILL";
                            dtl.DocId = docId;
                            dtl.DocNo = response.BillNo!;
                            dtl.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            dtl.DtlRecId = d.IdNo;
                            dtl.InQty = data.PurchaseQty;
                            dtl.OutQty = 0;
                            dtl.Rate = data.Rate;
                            dtl.Mrp = 0;
                            dtl.Amount = (data.PurchaseQty * data.Rate);
                            await context.AddAsync(dtl);
                            await context.SaveChangesAsync();

                            /****************************************** adjust purorderdtl ************************************************/
                            if(data.PODtlIdNo > 0)
                            {
                                var existingPODtl = await context.PurOrderDtls.FirstOrDefaultAsync(m => m.IdNo == data.PODtlIdNo);
                                existingPODtl!.PbQtyAdj = (existingPODtl.PbQtyAdj ?? 0) +  data.PurchaseQty;
                                context.PurOrderDtls.Update(existingPODtl);
                                await context.SaveChangesAsync();
                            } // end if...                            
                            /**************************************************************************************************************/
                        } // end of foreach loop...                        
                    } // New Mode...
                    else
                    {
                        docId = response.BillId;

                        List<AccountsPo> posDelete = await context.AccountsPos.Where(m => m.DocId == response.BillId
                        && m.ModuleName == "PURCHASE BILL").ToListAsync();
                        context.AccountsPos.RemoveRange(posDelete);
                        await context.SaveChangesAsync();

                        List<StockDtl> stocks = await context.StockDtls.Where(m => m.DocId == docId && m.ModuleName == "PURCHASE BILL").ToListAsync();
                        context.StockDtls.RemoveRange(stocks);
                        await context.SaveChangesAsync();

                        /*********************************************** delete from PurBillDtl ***************************************************/
                        List<PurBillDtl> dtls = await context.PurBillDtls.Where(m => m.BillId == response.BillId).ToListAsync();
                        List<int> incomingIds = response.ListPBDtls!.Where(m => m.IdNo > 0).Select(m => m.IdNo).ToList();
                        List<PurBillDtl> deletedRecords = dtls.Where(x => !incomingIds.Contains(x.IdNo)).ToList();
                        context.PurBillDtls.RemoveRange(deletedRecords);
                        await context.SaveChangesAsync();
                        /**************************************************************************************************************************/
                        var existingHead = await context.PurBillHeads.FirstOrDefaultAsync(m => m.BillId == response.BillId);
                        existingHead!.BillNo = response.BillNo!;
                        existingHead.BillDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        existingHead.AccountId = (int)response.LedgerId!;
                        existingHead.PartyCode = response.PartyCode;
                        existingHead.NetAmount = (decimal)response.ListPBDtls!.Sum(m => m.AmountAfterDiscount)!;
                        existingHead.AccountsAdjAmount = b ? existingHead.NetAmount : 0.00m;
                        existingHead.Remarks = response.Remarks ?? string.Empty;
                        context.Update(existingHead);
                        await context.SaveChangesAsync();

                        foreach (var data in response.ListPBDtls!)
                        {
                            if(data.IdNo == 0)
                            {
                                PurBillDtl d = new PurBillDtl();
                                d.BillId = response.BillId;
                                d.BillNo = response.BillNo!;
                                d.BillDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                d.ItemId = data.ItemId;
                                d.Qty = data.PurchaseQty;
                                d.Rate = data.Rate;
                                d.Amount = data.Amount;
                                d.Cgst = data.CGST;
                                d.Sgst = data.SGST;
                                d.Igst = (data.CGST + data.SGST);
                                d.CgstledgerId = cgstAccountId;
                                d.SgstledgerId = sgstAccountId;
                                d.IgstledgerId = igstAccountId;
                                d.DiscountPerc = data.DiscountPerc;
                                d.DiscountValue = data.DiscountValue;
                                d.DiscountLedgerId = data.DiscountValue == 0 ? null : discountAccountId;
                                d.TotalAmount = (data.Amount + data.CGST + data.SGST);
                                d.AmountAfterDiscount = data.AmountAfterDiscount;
                                d.PodtlIdNo = data.PODtlIdNo == 0 ? null : data.PODtlIdNo;
                                d.OrderId = data.OrderId == 0 ? null : data.OrderId;
                                await context.AddAsync(d);
                                await context.SaveChangesAsync();

                                StockDtl dtl = new StockDtl();
                                dtl.ItemId = data.ItemId;
                                dtl.ModuleName = "PURCHASE BILL";
                                dtl.DocId = docId;
                                dtl.DocNo = response.BillNo!;
                                dtl.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                dtl.DtlRecId = d.IdNo;
                                dtl.InQty = data.PurchaseQty;
                                dtl.OutQty = 0;
                                dtl.Rate = data.Rate;
                                dtl.Mrp = 0;
                                dtl.Amount = (data.PurchaseQty * data.Rate);
                                await context.AddAsync(dtl);
                                await context.SaveChangesAsync();

                                /****************************************** adjust purorderdtl ************************************************/
                                if (data.PODtlIdNo > 0)
                                {
                                    var existingPODtl = await context.PurOrderDtls.FirstOrDefaultAsync(m => m.IdNo == data.PODtlIdNo);
                                    existingPODtl!.PbQtyAdj = (existingPODtl.PbQtyAdj ?? 0) + data.PurchaseQty;
                                    context.PurOrderDtls.Update(existingPODtl);
                                    await context.SaveChangesAsync();
                                } // end if...    
                            }
                            else
                            {
                                var existingDtl = await context.PurBillDtls.FirstOrDefaultAsync(m => m.BillId == response.BillId && m.IdNo == data.IdNo);
                                int beforeUpdateQty = existingDtl!.Qty;

                                existingDtl!.BillNo = response.BillNo!;
                                existingDtl.BillDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                existingDtl.ItemId = data.ItemId;
                                existingDtl.Qty = data.PurchaseQty;
                                existingDtl.Rate = data.Rate;
                                existingDtl.Amount = data.Amount;
                                existingDtl.Cgst = data.CGST;
                                existingDtl.Sgst = data.SGST;
                                existingDtl.Igst = (data.CGST + data.SGST);
                                existingDtl.CgstledgerId = cgstAccountId;
                                existingDtl.SgstledgerId = sgstAccountId;
                                existingDtl.IgstledgerId = igstAccountId;
                                existingDtl.DiscountPerc = data.DiscountPerc;
                                existingDtl.DiscountValue = data.DiscountValue;
                                existingDtl.DiscountLedgerId = data.DiscountValue == 0 ? null : discountAccountId;
                                existingDtl.TotalAmount = (data.Amount + data.CGST + data.SGST);
                                existingDtl.AmountAfterDiscount = data.AmountAfterDiscount;
                                context.Update(existingDtl);
                                await context.SaveChangesAsync();

                                StockDtl dtl = new StockDtl();
                                dtl.ItemId = data.ItemId;
                                dtl.ModuleName = "PURCHASE BILL";
                                dtl.DocId = docId;
                                dtl.DocNo = response.BillNo!;
                                dtl.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                dtl.DtlRecId = data.IdNo;
                                dtl.InQty = data.PurchaseQty;
                                dtl.OutQty = 0;
                                dtl.Rate = data.Rate;
                                dtl.Mrp = 0;
                                dtl.Amount = (data.PurchaseQty * data.Rate);
                                await context.AddAsync(dtl);
                                await context.SaveChangesAsync();

                                /****************************************** adjust purorderdtl ************************************************/
                                if (data.PODtlIdNo > 0)
                                {
                                    var existingPODtl = await context.PurOrderDtls.FirstOrDefaultAsync(m => m.IdNo == data.PODtlIdNo);
                                    existingPODtl!.PbQtyAdj = (existingPODtl.PbQtyAdj - beforeUpdateQty) + data.PurchaseQty;
                                    context.PurOrderDtls.Update(existingPODtl);
                                    await context.SaveChangesAsync();
                                } // end if...    
                            } // end if...                            
                        } // foreach loop...
                    } // Edit Mode...

                    /*********************************************** Accounts Pos *************************************************************/
                    AccountsPo pos1 = new AccountsPo();
                    pos1.AccountId = purCode;
                    pos1.ModuleName = "PURCHASE BILL";
                    pos1.DocId = docId;
                    pos1.DocNo = response.BillNo;
                    pos1.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    pos1.DrCr = "Debit";
                    pos1.Debit = response.ListPBDtls.Sum(m => m.Amount);
                    pos1.Credit = 0;
                    pos1.IdentifierId = 1;
                    await context.AddAsync(pos1);
                    await context.SaveChangesAsync();

                    decimal totalCGST = (decimal)response.ListPBDtls.Sum(m => m.CGST)!;
                    decimal totalSGST = (decimal)response.ListPBDtls.Sum(m => m.SGST)!;
                    if (totalCGST + totalSGST > 0)
                    {
                        AccountsPo pos2 = new AccountsPo();
                        pos2.AccountId = cgstAccountId;
                        pos2.ModuleName = "PURCHASE BILL";
                        pos2.DocId = docId;
                        pos2.DocNo = response.BillNo;
                        pos2.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        pos2.DrCr = "Debit";
                        pos2.Debit = totalCGST;
                        pos2.Credit = 0;
                        pos2.IdentifierId = 1;
                        await context.AddAsync(pos2);
                        await context.SaveChangesAsync();

                        AccountsPo pos3 = new AccountsPo();
                        pos3.AccountId = sgstAccountId;
                        pos3.ModuleName = "PURCHASE BILL";
                        pos3.DocId = docId;
                        pos3.DocNo = response.BillNo;
                        pos3.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        pos3.DrCr = "Debit";
                        pos3.Debit = totalSGST;
                        pos3.Credit = 0;
                        pos3.IdentifierId = 1;
                        await context.AddAsync(pos3);
                        await context.SaveChangesAsync();
                    }

                    AccountsPo c1 = new AccountsPo();
                    c1.AccountId = response.LedgerId;
                    c1.PartyCode = response.PartyCode;
                    c1.ModuleName = "PURCHASE BILL";
                    c1.DocId = docId;
                    c1.DocNo = response.BillNo;
                    c1.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    c1.DrCr = "Credit";
                    c1.Debit = 0;
                    c1.Credit = response.ListPBDtls.Sum(m => m.AmountAfterDiscount);
                    c1.IdentifierId = 2;
                    await context.AddAsync(c1);
                    await context.SaveChangesAsync();

                    if (response.ListPBDtls.Sum(m => m.DiscountValue) > 0)
                    {
                        AccountsPo c2 = new AccountsPo();
                        c2.AccountId = discountAccountId;
                        c2.ModuleName = "PURCHASE BILL";
                        c2.DocId = docId;
                        c2.DocNo = response.BillNo;
                        c2.DocDate = DateOnly.ParseExact(response.BillDate!, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        c2.DrCr = "Credit";
                        c2.Debit = 0;
                        c2.Credit = response.ListPBDtls.Sum(m => m.DiscountValue);
                        c2.IdentifierId = 2;
                        await context.AddAsync(c2);
                        await context.SaveChangesAsync();
                    } // end if...
                    /**************************************************************************************************************************/                    
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
            } // SaveBill...

            public async Task<List<PurchaseBillDtlResponse>> PopulatePBByOrderId(int[] orderIds)
            {
                string sql = @"select	0 IdNo, d.ItemId, i.ItemName, isnull(d.Qty,0) OrderQty, isnull(d.PbQtyAdj,0) AdjustedQty,  
		                                (isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) PurchaseQty, 0 BalanceQty, d.Rate, 
		                                (isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate Amount, 
		                                (((select PurCGSTPerc from ItemGST where ItemId = d.ItemId)/100) * ((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate)) CGST,
		                                (((select PurSGSTPerc from ItemGST where ItemId = d.ItemId)/100) * ((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate)) SGST,
		                                (((select PurIGSTPerc from ItemGST where ItemId = d.ItemId)/100) * d.Amount) IGST,
		                                0 DiscountPerc, 0 DiscountValue,
		                                ((((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate)) + 
		                                (((select PurCGSTPerc from ItemGST where ItemId = d.ItemId)/100) * (((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate))) + 
		                                (((select PurSGSTPerc from ItemGST where ItemId = d.ItemId)/100) * (((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate)))) TotalAmount,
		                                ((((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate)) + 
		                                (((select PurCGSTPerc from ItemGST where ItemId = d.ItemId)/100) * (((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate))) + 
		                                (((select PurSGSTPerc from ItemGST where ItemId = d.ItemId)/100) * (((isnull(d.Qty,0) - isnull(d.PbQtyAdj,0)) * d.Rate)))) AmountAfterDiscount,
		                                d.IdNo PODtlIdNo, d.OrderId, d.OrderNo OrderNo
                                  from PurOrderDtl d inner join ItemHead i on (d.ItemId = i.ItemId)
                                 where d.OrderId IN @ids and isnull(d.Qty,0) - isnull(d.PbQtyAdj,0) > 0 order by d.OrderNo";

                var dtls = await db.QueryAsync<PurchaseBillDtlResponse>(sql, new { @ids = orderIds});
                return dtls.ToList();
            } // PopulatePBByOrderId...

            public async Task<PurchaseBillDtlResponse> GetPBDtlByPBDtlRecid(int recId)
            {
                string str = @"select d.IdNo, d.ItemId, i.ItemName, od.Qty OrderQty, isnull(od.PbQtyAdj,0) AdjustedQty, d.Qty PurchaseQty,
                                      (isnull(od.Qty,0) - isnull(od.PbQtyAdj,0)) BalanceQty, d.Rate, d.Amount, 
                                      d.CGST, d.SGST, d.IGST, d.DiscountPerc, d.DiscountValue, d.TotalAmount, d.AmountAfterDiscount, 
                                      d.PODtlIdNo, d.OrderId, od.OrderNo
                                  from PurBillDtl d inner join ItemHead i on (i.ItemId = d.ItemId)
                                  left join PurOrderDtl od on (d.PODtlIdNo = od.IdNo)
                                 where d.IdNo = @id";

                var dtl = await db.QueryFirstOrDefaultAsync<PurchaseBillDtlResponse>(str, new { id = recId}) ?? new PurchaseBillDtlResponse() { PurchaseQty = 0};
                return dtl;
            } // GetPBDtlByPBDtlRecid...
        } // class...
    } // namespace NPurchaseBill...
}
