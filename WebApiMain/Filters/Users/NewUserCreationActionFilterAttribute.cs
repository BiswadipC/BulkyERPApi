using Domain.UserAuthentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.Users
{
    public class NewUserCreationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var user = context.ActionArguments["user"] as UserResponse;

            if(string.IsNullOrEmpty(user!.UserName))
            {
                context.ModelState.AddModelError("BadRequest", "Username cannot be blank.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if (string.IsNullOrEmpty(user!.Password))
            {
                context.ModelState.AddModelError("BadRequest", "Password cannot be blank.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if (string.IsNullOrEmpty(user!.ReTypePassword))
            {
                context.ModelState.AddModelError("BadRequest", "Re-Type Password cannot be blank.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if (user!.Password != user!.ReTypePassword)
            {
                context.ModelState.AddModelError("BadRequest", "Both Password and Re-Type Password must be same.");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if(string.IsNullOrEmpty(user!.Mobile))
            {
                context.ModelState.AddModelError("BadRequest", "Mobile No. cannot be blank");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }

            if (string.IsNullOrEmpty(user!.Email))
            {
                context.ModelState.AddModelError("BadRequest", "Email cannot be blank");
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
