using Domain.Accounts;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.Accounts
{
    public class AccountsApiActionFilter : ActionFilterAttribute
    {
        private readonly BulkyContext bulky;

        public AccountsApiActionFilter(BulkyContext bulky)
        {
            this.bulky = bulky;
        } // constructor...

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var Id = context.ActionArguments["id"] as int?;
            if(!Id.HasValue || Id.Value <= 0)
            {
                context.ModelState.AddModelError("Account", "Invalid or null Accounts Id. specified.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            var response = (from b in bulky.Accounts
                            select new AccountsResponse
                            {
                                AccountId = b.AccountId,
                                AccountName = b.AccountName,
                                Category = b.Category,
                                Schedule = b.Schedule,
                                TaxStructure = b.TaxStructure,
                                Add1 = b.Add1 ?? string.Empty,
                                City = b.City ?? string.Empty,
                                State = b.State ?? string.Empty,
                                Pin = b.Pin ?? string.Empty,
                                Phone = b.Phone ?? string.Empty,
                                Mobile = b.Mobile ?? string.Empty,
                                Email = b.Email ?? string.Empty,
                                Website = b.Website ?? string.Empty,
                                AccountNo = b.AccountNo ?? string.Empty,
                                IFSCCode = b.Ifsccode ?? string.Empty,
                                BranchCode = b.BranchCode ?? string.Empty
                            }).FirstOrDefault(m => m.AccountId == Id) as AccountsResponse;
            if (response == null)
            {
                context.ModelState.AddModelError("Account", $"Accounts Id. {Id} is invalid. No records fetched.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status404NotFound
                };
                context.Result = new NotFoundObjectResult(problemDetails);
                return;
            }

            context.HttpContext.Items["response"] = response;
        } // OnActionExecuting...
    } // class...
}
