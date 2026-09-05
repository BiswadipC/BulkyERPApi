using Domain.ItemMaster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.ItemMaster
{
    public class SaveItemActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var head = context.ActionArguments["head"] as ItemHeadResponse;
            if(head == null)
            {
                context.ModelState.AddModelError("BadRequest", "Invalid Request.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if (string.IsNullOrWhiteSpace(head.ItemName))
            {
                context.ModelState.AddModelError("BadRequest", "Enter an Item Name.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if(head.ListItemDtlResponse  != null && head.ListItemDtlResponse.Count() > 0)
            {
                head.ListItemDtlResponse.ForEach(m =>
                {
                    if(m.AttrHeadIdNo.HasValue && !m.AttrDtlIdNo.HasValue)
                    {
                        context.ModelState.AddModelError("BadRequest", "An Attribute must have a value.");
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status400BadRequest
                        };
                        context.Result = new BadRequestObjectResult(problemDetails);
                        return;
                    }
                });
            } // end if...
        } // OnActionExecuting...
    } // class...
}
