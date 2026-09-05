using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.ItemMaster
{
    public class GetItemHeadByIdActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var id = context.ActionArguments["id"] as int?;
            if(id == null || id <= 0)
            {
                context.ModelState.AddModelError("BadRequest", "Invalid ItemId Provided.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            base.OnActionExecuting(context);
        } // OnActionExecuting...
    } // class...
}
