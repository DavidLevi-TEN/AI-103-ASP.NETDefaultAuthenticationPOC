using ASP.NETDefaultAuthenticationPOC.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;


namespace ASP.NETDefaultAuthenticationPOC.Authorization;


public class AdminstratorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Contact resource)
    {
        if (context.User == null)
        {
            // v Neither success nor failure, simply allows other handlers to run
            return Task.CompletedTask;
        }

        // Adminstrators can do anything
        if (context.User.IsInRole(Constants.ContactAdministratorsRole))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
