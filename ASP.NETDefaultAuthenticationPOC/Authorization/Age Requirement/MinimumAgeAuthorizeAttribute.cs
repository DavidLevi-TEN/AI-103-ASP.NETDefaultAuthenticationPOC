using Microsoft.AspNetCore.Authorization;

namespace ASP.NETDefaultAuthenticationPOC.Authorization.AgeRequirement;


public class MinimumAgeAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
{
    public MinimumAgeAuthorizeAttribute(int age) => Age = age;
    public int Age { get; set; }

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}