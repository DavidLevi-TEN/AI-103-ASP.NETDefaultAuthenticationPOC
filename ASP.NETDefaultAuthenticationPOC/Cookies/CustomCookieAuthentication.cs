using Microsoft.AspNetCore.Authentication.Cookies;
using Umbraco.Cms.Core.Security;


namespace ASP.NETDefaultAuthenticationPOC.Cookies;


public class CustomCookieAuthentication : CookieAuthenticationEvents
{
    private readonly IUserRepository _respository;


    public CustomCookieAuthentication(IUserRepository repository)
    {
        _respository = repository;
    }


    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userPrincipal = context.Principal;

        var lastChanged = (from c in userPrincipal.Claims
                           where c.Type == "LastChanged"
                           select c.Value).FirstOrDefault();

        if (string.IsNullOrEmpty(lastChanged) || !_respository.ValidateLastChanged(lastChanged))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
