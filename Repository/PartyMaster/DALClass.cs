using Domain.PartyMaster;
using m = Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Models;

namespace Repository.PartyMaster
{
    namespace NPartyMaster
    {
        internal class DALClass : IPartyMaster
        {
            private readonly BulkyContext context;

            public DALClass(BulkyContext context)
            {
                this.context = context;
            } // constructor...

            public async Task<List<PartyResponse>> GetParties()
            {
                var parties = await context.PartyMasters.Select(x => new PartyResponse
                {
                    PartyCode = x.PartyCode,
                    PartyName = x.PartyName,
                    Add1 = x.Add1,
                    Add2 = x.Add2,
                    City = x.City,
                    State = x.State,
                    Pin = x.Pin,
                    Mobile = x.Mobile,
                    GSTNo = x.Gstno,
                    DrugLicenceNo = x.DrugLicenceNo
                }).ToListAsync();

                return parties;
            } // GetParty...

            public async Task<PartyResponse> GetPartyByCode(int code)
            {
                return await context.PartyMasters.Where(m => m.PartyCode == code)
                            .Select(x => new PartyResponse
                            {
                                PartyCode = x.PartyCode,
                                PartyName = x.PartyName,
                                Add1 = x.Add1,
                                Add2 = x.Add2,
                                City = x.City,
                                State = x.State,
                                Pin = x.Pin,
                                Mobile = x.Mobile,
                                GSTNo = x.Gstno,
                                DrugLicenceNo = x.DrugLicenceNo
                            }).FirstOrDefaultAsync() ?? new PartyResponse() { PartyCode = 0 };
            } // GetPartyByCode...

            public async Task<string> Save(PartyResponse response)
            {
                string message = string.Empty;
                var trans = await context.Database.BeginTransactionAsync();

                try
                {
                    if(context.PartyMasters.Any(x => x.PartyName.ToLower() == response.PartyName.ToLower() && response.PartyCode == 0))
                    {
                        message = $"Party {response.PartyName}, already exists.";
                        return message;
                    }

                    if(context.PartyMasters.Any(x => x.PartyName.ToLower() == response.PartyName.ToLower() && response.PartyCode != x.PartyCode))
                    {
                        message = $"Party {response.PartyName}, already exists.";
                        return message;
                    }

                    if(response.PartyCode == 0)
                    {
                        m.PartyMaster party = new m.PartyMaster();
                        party.PartyName = response.PartyName;
                        party.Add1 = response.Add1 ?? string.Empty;
                        party.Add2 = response.Add2 ?? string.Empty;
                        party.City = response.City ?? string.Empty;
                        party.State = response.State ?? string.Empty;
                        party.Pin = response.Pin ?? string.Empty;
                        party.Mobile = response.Mobile ?? string.Empty;
                        party.Gstno = response.GSTNo ?? string.Empty;
                        party.DrugLicenceNo = response.DrugLicenceNo ?? string.Empty;

                        await context.AddAsync(party);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        var existingParty = await context.PartyMasters.FirstOrDefaultAsync(m => m.PartyCode == response.PartyCode);
                        if (existingParty != null)
                        {
                            existingParty.PartyName = response.PartyName;
                            existingParty.Add1 = response.Add1 ?? string.Empty;
                            existingParty.Add2 = response.Add2 ?? string.Empty;
                            existingParty.City = response.City ?? string.Empty;
                            existingParty.State = response.State ?? string.Empty;
                            existingParty.Pin = response.Pin ?? string.Empty;
                            existingParty.Mobile = response.Mobile ?? string.Empty;
                            existingParty.Gstno = response.GSTNo ?? string.Empty;
                            existingParty.DrugLicenceNo = response.DrugLicenceNo ?? string.Empty;

                            context.Update(existingParty);
                            await context.SaveChangesAsync();
                        }
                    } // end if...

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
    } // namespace NPartyMaster...
}
