using Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Accounts
{
    public interface IAccountsResponse
    {
        Task<List<AccountsResponse>> GetAccounts();
        Task<AccountsResponse> GetAccountById(int id);
        Task<string> Save(AccountsResponse response);
    } // interface...
}
