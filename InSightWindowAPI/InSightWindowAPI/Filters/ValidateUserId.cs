using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using InSightWindowAPI.Extensions;

namespace InSightWindowAPI.Filters
{
    public class ValidateUserIdAsyncAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _callerName;

        public ValidateUserIdAsyncAttribute(string callerName)
        {
            _callerName = callerName;
          
        }
      
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {   
            //before
            
            Guid userId = context.HttpContext.GetUserIdFromClaims(); 
            if (userId == Guid.Empty)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
            //after
        }
    }
}


