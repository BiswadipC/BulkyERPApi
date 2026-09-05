using Azure;
using Domain.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Accounts;
using WebApiMain.Filters.Accounts;

namespace WebApiMain.Controllers
{
    [Route("accounts")]
    [ApiController]    

    public class AccountsApiController : ControllerBase
    {
        private readonly IAccountsResponse iaccounts;
        private readonly IAccountsCategoryMasterResponse icategories;

        public AccountsApiController(IAccountsResponse iaccounts, IAccountsCategoryMasterResponse icategories)
        {
            this.iaccounts = iaccounts;
            this.icategories = icategories;
        } // constructor...

        [Authorize(Policy = "LEDGERMASTER-VIEW_POLICY")]
        [HttpGet("")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await iaccounts.GetAccounts();
            return Ok(accounts);
        } // GetAccounts...

        [HttpGet("{id}")]
        [TypeFilter(typeof(AccountsApiActionFilter))]
        [Authorize(Policy = "LEDGERMASTER-ALL_POLICY")]
        public async Task<IActionResult> GetAccountById(int? id)
        {
            var account = HttpContext.Items["response"] as AccountsResponse;
            return await Task.Run(() =>
            {
                return Ok(account);
            });
        } // GetAccountById...

        [HttpPost("")]
        [Authorize(Policy = "LEDGERMASTER-ALL_POLICY")]
        public async Task<IActionResult> Save(AccountsResponse response)
        {
            string message = await iaccounts.Save(response);
            if(message == "Success")
            {
                return Ok(message);
            }

            return BadRequest(message);
        } // Save...

        /*******************************************************************************************************************************/
        [HttpGet("categories")]
        public async Task<IActionResult> GetAccountsCategoryMasterResponses()
        {
            var categories = await icategories.GetAccountsCategoryMasterResponses();
            return Ok(categories);
        } // GetAccountsCategoryMasterResponses...
    } // class...
}
