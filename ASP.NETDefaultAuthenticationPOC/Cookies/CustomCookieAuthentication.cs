using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Umbraco.Cms.Core.Persistence.Repositories;


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

        var lastChanged = (from c in userPrincipal!.Claims
                           where c.Type == "LastChanged"
                           select c.Value).FirstOrDefault();

        if (lastChanged != null)
        {
            var user = _respository.GetProfile(userPrincipal.Identity!.Name!);

            if (user == null || user.GetType().ToString() != lastChanged)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
