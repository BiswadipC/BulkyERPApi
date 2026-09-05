using Domain.PartyMaster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiMain.Filters.PartyMaster
{
    public class SavePartyMasterActionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var party = context.ActionArguments["party"] as PartyResponse;
            if(party != null)
            {
                if(string.IsNullOrWhiteSpace(party.PartyName))
                {
                    context.ModelState.AddModelError("BadRequest", "Part Name cannot be blank.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                    //return;
                }

                if(!string.IsNullOrWhiteSpace(party.Mobile))
                {
                    if(party.Mobile.Length != 10)
                    {
                        context.ModelState.AddModelError("BadRequest", "Mobile No. must be of 10 digits length.");
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status400BadRequest
                        };
                        context.Result = new BadRequestObjectResult(problemDetails);
                        //return;
                    }

                    bool b = party.Mobile.All(x => char.IsDigit(x));
                    if(!b)
                    {
                        context.ModelState.AddModelError("BadRequest", "Mobile No. must contain all numeric digits \'(0-9)\'.");
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status400BadRequest
                        };
                        context.Result = new BadRequestObjectResult(problemDetails);
                        //return;
                    }

                    if(party.Mobile.StartsWith("0"))
                    {
                        context.ModelState.AddModelError("BadRequest", "Mobile No. cannot start with \'0\'.");
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status400BadRequest
                        };
                        context.Result = new BadRequestObjectResult(problemDetails);
                        //return;
                    }
                }
            } // end if...
        } // OnActionExecuting...
    } // class...
}
