using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;


namespace ContactManager.Authorization;


public class OwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
{
    UserManager<IdentityUser> _userManager;

    
    public OwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }


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

        // Return if it's not asking for CRUD permission
        if (requirement.Name != Constants.CreateOperationName &&
            requirement.Name != Constants.ReadOperationName &&
            requirement.Name != Constants.UpdateOperationName &&
            requirement.Name != Constants.DeleteOperationName)
        {
            return Task.CompletedTask;
        }

        if (resource.OwnerId == _userManager.GetUserId(context.User))
        {
            return context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
