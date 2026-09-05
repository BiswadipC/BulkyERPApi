using Domain.ItemAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.ItemAttributes
{
    public class ItemAttrsSaveActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var head = context.ActionArguments["head"] as ItemAttrHeadResponse;
            if(head == null)
            {
                context.ModelState.AddModelError("NotFound", "No data found.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status404NotFound
                };
                context.Result = new NotFoundObjectResult(problemDetails);
                return;
            }

            if(string.IsNullOrWhiteSpace(head!.AttrName))
            {
                context.ModelState.AddModelError("BadRequest", "Attribute Name cannot be blank.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if(head.ListAttrDtls == null || head.ListAttrDtls.Count() == 0)
            {
                context.ModelState.AddModelError("BadRequest", "An attribute must have atleast one value.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }
        } // OnActionExecuting...
    } // class...
}
