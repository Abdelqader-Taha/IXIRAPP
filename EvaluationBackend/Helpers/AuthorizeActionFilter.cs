using System.Security.Claims;
using EvaluationBackend.Repository;
using EvaluationBackend.DATA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using TextExtensions;

namespace EvaluationBackend.Helpers
{
    public class AuthorizeActionFilter : IAsyncActionFilter
    {
        
        private readonly DataContext _dbContext;

        public AuthorizeActionFilter(DataContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                string controllerName = controllerActionDescriptor.ControllerName;
                string actionName = GetCrudType(controllerActionDescriptor);

                string requiredPermission = $"{controllerName.ToKebabCase()}.{actionName.ToKebabCase()}";


              
            }

            await next();
        }
        
        
        private string GetCrudType(ControllerActionDescriptor descriptor)
        {
            return descriptor.ActionName.ToKebabCase();
         
        }
        
    }
}