using Domain.ItemAttributes;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ItemAttributes
{
    namespace NItemAttributes
    {
        internal sealed class DALClass : IItemAttributeResponse
        {
            private readonly BulkyContext context;

            public DALClass(BulkyContext context)
            {
                this.context = context;
            } // constructor...

            public async Task<List<ItemAttrHeadResponse>> GetAllAttributes()
            {
                return await
                (from ac in context.AttrHeads
                 select new ItemAttrHeadResponse
                 {
                     IdNo = ac.IdNo,
                     AttrName = ac.AttrName
                 }).ToListAsync();
            } // GetAllAttributes...

            public async Task<List<ItemAttrDtlresponse>> GetAttrDtlsByAttrHeadId(int headId)
            {
                return await
                (from ad in context.Attrdtls
                 where ad.AttrHeadIdNo == headId
                 select new ItemAttrDtlresponse
                 {
                     IdNo = ad.IdNo,
                     AttrValue = ad.AttrValue,
                 }).ToListAsync();
            } // GetAttrDtlsByAttrHeadId...

            public async Task<ItemAttrHeadResponse> GetAttributeById(int? id)
            {
                ItemAttrHeadResponse? attrHead = await (from cs in context.AttrHeads
                                                       where cs.IdNo == id
                                                       select new ItemAttrHeadResponse
                                                       {
                                                           IdNo = id.HasValue ? id.Value : 0,
                                                           AttrName = cs.AttrName
                                                       }).FirstOrDefaultAsync();

                if(attrHead == null)
                {
                    return new ItemAttrHeadResponse() { IdNo = 0 };
                }

                List<ItemAttrDtlresponse> dtls = await GetAttrDtlsByAttrHeadId(id.HasValue ? id.Value : 0);
                attrHead.ListAttrDtls = dtls;

                return attrHead;
            } // GetAttributeById...

            public async Task<string> Save(ItemAttrHeadResponse head)
            {
                string message = string.Empty;
                var trans = await context.Database.BeginTransactionAsync();

                try
                {
                    if(head.IdNo == 0)
                    {
                        AttrHead h = new AttrHead();
                        h.AttrName = head.AttrName!;
                        await context.AddAsync(h);
                        await context.SaveChangesAsync();

                        foreach(var dtl in head.ListAttrDtls)
                        {
                            Attrdtl d = new Attrdtl();
                            d.AttrHeadIdNo = h.IdNo;
                            d.AttrValue = dtl.AttrValue ?? string.Empty;
                            await context.AddAsync(d);
                            await context.SaveChangesAsync();
                        }                        
                    } // insert mode...
                    else
                    {
                        var existingHead = await context.AttrHeads.FirstOrDefaultAsync(m => m.IdNo == head.IdNo);
                        existingHead!.AttrName = head.AttrName;
                        context.Update(existingHead);
                        await context.SaveChangesAsync();

                        foreach (var dtl in head.ListAttrDtls)
                        {
                            if(dtl.IdNo == 0)
                            {
                                Attrdtl d = new Attrdtl();
                                d.AttrHeadIdNo = head.IdNo;
                                d.AttrValue = dtl.AttrValue ?? string.Empty;
                                await context.AddAsync(d);
                                await context.SaveChangesAsync();
                            }
                            else
                            {
                                var existingDtl = await context.Attrdtls.FirstOrDefaultAsync(m => m.IdNo == dtl.IdNo);
                                existingDtl!.AttrValue = dtl.AttrValue ?? string.Empty;
                                context.Update(existingDtl);
                                await context.SaveChangesAsync();
                            } // end if...                            
                        }
                    } // edit mode...

                    await trans.CommitAsync();
                    message = "Success";
                } // end of try...
                catch(Exception ex)
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
    } // namespace NItemAttributes...
}
