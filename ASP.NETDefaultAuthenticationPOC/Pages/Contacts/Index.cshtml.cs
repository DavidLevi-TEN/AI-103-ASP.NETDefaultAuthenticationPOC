using ASP.NETDefaultAuthenticationPOC.Authorization;
using ASP.NETDefaultAuthenticationPOC.Data;
using ASP.NETDefaultAuthenticationPOC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ASP.NETDefaultAuthenticationPOC.Pages.Contacts;


public class IndexModel : BasePageModel
{
    public IndexModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
    {
    }


    public IList<Contact> Contact { get;set; }


    public async Task OnGetAsync()
    {
        var contacts = from c in Context.Contact select c;

        var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Context, ContactOperations.Read);

        var currentUserId = UserManager.GetUserId(User);

        // Only the owner and authorized users can see approved contacts
        if (isAuthorized.Succeeded)
        {
            contacts = contacts.Where(c => c.Status == ContactStatus.Approved || c.OwnerId == currentUserId);
        }

        Contact = await contacts.ToListAsync();
    }
}
