using ContactManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace ContactManager.Pages.Contacts;


public class BasePageModel : PageModel
{
    protected ApplicationDbContext Context { get; }
    protected IAuthorizationService AuthorizationService { get; }
    protected UserManager<IdentityUser> UserManager { get; }


    public BasePageModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
    {
        Context = context;
        AuthorizationService = authorizationService;
        UserManager = userManager;
    }
}
