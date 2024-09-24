using Microsoft.AspNetCore.Authorization;


namespace ASP.NETDefaultAuthenticationPOC.Authorization.AgeRequirement;


public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;

    public int MinimumAge { get; }
}
