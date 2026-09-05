using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.ItemAttributes
{
    public class ItemAttrsActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var Id = context.ActionArguments["id"] as int?;
            if(!Id.HasValue)
            {
                context.ModelState.AddModelError("IdNotFound", "Item Attribute Head Id. must have a value.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status404NotFound
                };
                context.Result = new NotFoundObjectResult(problemDetails);
                return;
            } // end if...
            
            if(Id.Value <= 0)
            {
                context.ModelState.AddModelError("InvalidId", "Invalid Id. Check your Attribute Head Id.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            } // end if...
        } // OnActionExecuting...
    } // class...
}
