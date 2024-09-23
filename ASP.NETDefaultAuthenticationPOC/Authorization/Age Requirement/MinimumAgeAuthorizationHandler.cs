using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Security.Claims;


namespace ASP.NETDefaultAuthenticationPOC.Authorization.AgeRequirement;


public class MinimumAgeAuthorizationHandler : AuthorizationHandler<MinimumAgeAuthorizeAttribute>
{
    private readonly ILogger<MinimumAgeAuthorizationHandler> _logger;


    public MinimumAgeAuthorizationHandler(ILogger<MinimumAgeAuthorizationHandler> logger)
    {
        _logger = logger;
    }


    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeAuthorizeAttribute requirement)
    {
        _logger.LogWarning("Checking if user's age of {age} passes requirement...", requirement.Age);

        var dateOfBirthClaim = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth);

        if (dateOfBirthClaim != null)
        {
            var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value, CultureInfo.InvariantCulture);

            var age = DateTime.Today.Year - dateOfBirth.Year;

            if (dateOfBirth > DateTime.Today.AddYears(-age))
            {
                age--; // In case the user hasn't had their birthday yet this year.
            }

            if (age >= requirement.Age)
            {
                _logger.LogInformation("User meets the age requirement.");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation("User's claim of {ageClaim} does not meet the requirement of {requirement}.",
                    dateOfBirthClaim, requirement.Age);
            }
        }
        else
        {
            _logger.LogInformation("User does not have an age claim.");
        }

        return Task.CompletedTask;
    }
}
