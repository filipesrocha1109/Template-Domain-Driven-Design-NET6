using template.crosscutting.Utils;
using template.domain.Common;
using template.domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace template.api.Filters
{
    public class ValidationFilterAttribute : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Do something before the action executes.
            if (!context.ModelState.IsValid)
            {

                var allErrors = context.ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)
                                        .ToList();

                var response = new Response()
                {
                    Data = new { errors = allErrors },
                    Code = (int)ReturnCodes.InvalidValue,
                    Message = Utils.GetEnumDescription(ReturnCodes.InvalidValue),
                    Status = (int)ReturnStatus.BadRequest,
                    Success = false
                };

                context.Result = new BadRequestObjectResult(response);
                return;
            }

            await next();
            // Do something after the action executes.
        }
    }
}

