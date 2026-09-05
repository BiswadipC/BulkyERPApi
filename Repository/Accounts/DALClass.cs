using Domain.Accounts;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Accounts
{
    namespace NAccounts
    {
        internal class DALClass : IAccountsResponse, IAccountsCategoryMasterResponse
        {
            private readonly BulkyContext context;

            public DALClass(BulkyContext context)
            {
                this.context = context;
            } 

            public async Task<List<AccountsResponse>> GetAccounts()
            {
                return await              
                    (from cs in context.Accounts
                     select new AccountsResponse
                     {
                         AccountId = cs.AccountId,
                         AccountName = cs.AccountName,
                         Category = cs.Category,
                         Schedule = cs.Schedule,
                         TaxStructure = cs.TaxStructure,
                         Add1 = cs.Add1 ?? string.Empty,
                         City = cs.City ?? string.Empty,
                         State = cs.State ?? string.Empty,
                         Pin = cs.Pin ?? string.Empty,
                         Phone = cs.Phone ?? string.Empty,
                         Mobile = cs.Mobile ?? string.Empty,
                         Email = cs.Email ?? string.Empty,
                         Website = cs.Website ?? string.Empty,
                         AccountNo = cs.AccountNo ?? string.Empty,
                         IFSCCode = cs.Ifsccode ?? string.Empty,
                         BranchCode = cs.BranchCode ?? string.Empty
                     }).ToListAsync();
            } // GetAccounts...

            public async Task<AccountsResponse> GetAccountById(int id)
            {
                return await
                    (from cs in context.Accounts
                     select new AccountsResponse
                     {
                         AccountId = cs.AccountId,
                         AccountName = cs.AccountName,
                         Category = cs.Category,
                         Schedule = cs.Schedule,
                         TaxStructure = cs.TaxStructure,
                         Add1 = cs.Add1 ?? string.Empty,
                         City = cs.City ?? string.Empty,
                         State = cs.State ?? string.Empty,
                         Pin = cs.Pin ?? string.Empty,
                         Phone = cs.Phone ?? string.Empty,
                         Mobile = cs.Mobile ?? string.Empty,
                         Email = cs.Email ?? string.Empty,
                         Website = cs.Website ?? string.Empty,
                         AccountNo = cs.AccountNo ?? string.Empty,
                         IFSCCode = cs.Ifsccode ?? string.Empty,
                         BranchCode = cs.BranchCode ?? string.Empty
                     }).FirstOrDefaultAsync(m => m.AccountId == id) ?? new AccountsResponse();
            } // GetAccountById...

            public async Task<string> Save(AccountsResponse response)
            {
                string message = string.Empty;

                if (string.IsNullOrWhiteSpace(response.AccountName))
                {
                    message = "Please specify an Account Name.";
                    return message;
                }

                if (string.IsNullOrWhiteSpace(response.Category))
                {
                    message = "Please specify an Account Category.";
                    return message;
                }

                if (string.IsNullOrWhiteSpace(response.Schedule))
                {
                    message = "Please specify an schedule.";
                    return message;
                }

                if (string.IsNullOrWhiteSpace(response.TaxStructure))
                {
                    message = "Tax Structure cannot be blank (specify eith GST or Non-GST).";
                    return message;
                }

                if(response.Category == "BA" && (string.IsNullOrEmpty(response.Add1) || string.IsNullOrEmpty(response.City) ||
                    string.IsNullOrEmpty(response.State) || string.IsNullOrEmpty(response.Pin) || string.IsNullOrEmpty(response.AccountNo)
                    || string.IsNullOrEmpty(response.IFSCCode) || string.IsNullOrEmpty(response.BranchCode) || string.IsNullOrEmpty(response.Mobile)))
                {
                    message = "Invalid bank informations. Please check your entry.";
                    return message;
                }

                if(response.Category != "BA")
                {
                    if(!string.IsNullOrEmpty(response.Add1) || !string.IsNullOrEmpty(response.City) || !string.IsNullOrEmpty(response.State) || !string.IsNullOrEmpty(response.Pin) ||
                        !string.IsNullOrEmpty(response.Phone) || !string.IsNullOrEmpty(response.Mobile) || !string.IsNullOrEmpty(response.Email) ||
                        !string.IsNullOrEmpty(response.Website) || !string.IsNullOrEmpty(response.AccountNo) || !string.IsNullOrEmpty(response.IFSCCode) ||
                        !string.IsNullOrEmpty(response.BranchCode))
                    {
                        message = "If \'Account Category\' is not \'Bank\', then other informations are invalid.";
                        return message;
                    }
                }
                /**********************************************************************************************************************************/
                var trans = await context.Database.BeginTransactionAsync();

                try
                {
                    if (response.AccountId == 0)
                    {
                        Account account = new Account();
                        account.AccountName = response.AccountName;
                        account.Category = response.Category;
                        account.Schedule = response.Schedule;
                        account.TaxStructure = response.TaxStructure;
                        account.Add1 = response.Add1 ?? string.Empty;
                        account.City = response.City ?? string.Empty;
                        account.State = response.State ?? string.Empty;
                        account.Pin = response.Pin ?? string.Empty;
                        account.Phone = response.Phone ?? string.Empty;
                        account.Mobile = response.Mobile ?? string.Empty;
                        account.Email = response.Email ?? string.Empty;
                        account.Website = response.Website ?? string.Empty;
                        account.AccountNo = response.AccountNo ?? string.Empty;
                        account.Ifsccode = response.IFSCCode ?? string.Empty;
                        account.BranchCode = response.BranchCode ?? string.Empty;
                        await context.AddAsync(account);
                    }
                    else
                    {
                        var existingAccount = context.Accounts.FirstOrDefault(m => m.AccountId == response.AccountId);
                        existingAccount!.AccountName = response.AccountName;
                        existingAccount!.Category = response.Category;
                        existingAccount!.Schedule = response.Schedule;
                        existingAccount!.TaxStructure = response.TaxStructure;
                        existingAccount!.Add1 = response.Add1;
                        existingAccount!.City = response.City;
                        existingAccount!.State = response.State;
                        existingAccount!.Pin = response.Pin;
                        existingAccount!.Phone = response.Phone;
                        existingAccount!.Mobile = response.Mobile;
                        existingAccount!.Email = response.Email;
                        existingAccount!.Website = response.Website;
                        existingAccount!.AccountNo = response.AccountNo;
                        existingAccount!.Ifsccode = response.IFSCCode;
                        existingAccount!.BranchCode = response.BranchCode;
                        context.Update(existingAccount);
                    } // end if...

                    await context.SaveChangesAsync();
                    await trans.CommitAsync();

                    message = "Success";
                } // end of try...
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    message = ex.ToString();
                } // exception...
                finally
                {
                    trans.Dispose();
                }
                
                return message;
            } // Save...

            /***********************************************************************************************************************************/
            public async Task<List<AccountsCategoryMasterResponse>> GetAccountsCategoryMasterResponses()
            {
                var categories = await (from cs in context.AccountsCategoryMasters
                                  select new AccountsCategoryMasterResponse
                                  {
                                      IdNo = cs.IdNo,
                                      CategoryCode = cs.CategoryCode,
                                      CategoryName = cs.CategoryName
                                  }).ToListAsync();

                return categories;
            } // GetAccountsCategoryMasterResponses...
        } // class...
    } // namespace NAccounts...
}
