using ContactManager.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;


namespace ContactManager.Authorization;


public class ManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Contact resource)
    {
        if (context.User == null || resource == null)
        {
            // v Neither success nor failure, simply allows other handlers to run
            return Task.CompletedTask;
        }

        // Return if it's not asking for approval or rejection
        if (requirement.Name != Constants.ApproveOperationName &&
            requirement.Name != Constants.RejectOperationName)
        {
            return Task.CompletedTask; 
        }

        // Managers can provide the above
        if (context.User.IsInRole(Constants.ContactManagersRole))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
