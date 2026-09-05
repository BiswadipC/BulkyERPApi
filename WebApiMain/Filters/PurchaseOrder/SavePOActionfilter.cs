using Domain.PurchaseOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.PurchaseOrder
{
    public class SavePOActionfilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var head = context.ActionArguments["head"] as PurchaseOrderHeadResponse;
            if(head != null)
            {
                if(string.IsNullOrWhiteSpace(head.OrderNo))
                {
                    context.ModelState.AddModelError("BadRequest", "Order No. cannot be blank.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                if (string.IsNullOrWhiteSpace(head.OrderDate))
                {
                    context.ModelState.AddModelError("BadRequest", "Order Date cannot be blank.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                bool b = DateOnly.TryParseExact(head.OrderDate, "dd/MM/yyyy", out DateOnly vdate);
                if(!b)
                {
                    context.ModelState.AddModelError("BadRequest", "Invalid Order Date format. Date should be in \'dd/mm/yyyy\' format.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                if(head.PartyCode == null || head.PartyCode == 0)
                {
                    context.ModelState.AddModelError("BadRequest", "Party Name cannot be blank.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                if(head.ListPoDtls == null || head.ListPoDtls.Count() == 0)
                {
                    context.ModelState.AddModelError("BadRequest", "Atleast one item entry should be there to save the record.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }

                return;
            } // end if...
        } // OnActionExecuting...
    } // class...
}
