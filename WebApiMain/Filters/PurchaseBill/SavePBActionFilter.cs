using Domain.PurchaseBill;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.PurchaseBill
{
    public class SavePBActionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var head = context.ActionArguments["head"] as PurchaseBillHeadResponse;
            if(head != null)
            {
                if(string.IsNullOrWhiteSpace(head.BillNo))
                {
                    context.ModelState.AddModelError("BadRequest", "Bill No. cannot be blank.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                if (string.IsNullOrWhiteSpace(head.BillDate))
                {
                    context.ModelState.AddModelError("BadRequest", "Bill Date cannot be blank.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                bool b = DateOnly.TryParseExact(head.BillDate, "dd/MM/yyyy", out var result);
                if (!b)
                {
                    context.ModelState.AddModelError("BadRequest", "Invalid Bill Date specified. Bill Date should be in \'dd/mm/yyyy\' format.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                if(head.LedgerId == null || head.LedgerId <= 0)
                {
                    context.ModelState.AddModelError("BadRequest", "Ledger Name must be specified.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                if (head.PartyCode <= 0)
                {
                    context.ModelState.AddModelError("BadRequest", "Invalid Party Name specified.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                if (head.ListPBDtls == null || head.ListPBDtls.Count() == 0)
                {
                    context.ModelState.AddModelError("BadRequest", "Atleast one item record must be specied to save the record.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }
            } // end if...
        } // OnActionExecuting...
    } // class...
}
