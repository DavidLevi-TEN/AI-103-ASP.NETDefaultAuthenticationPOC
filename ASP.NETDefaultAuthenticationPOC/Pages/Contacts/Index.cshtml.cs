using ContactManager.Authorization;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ContactManager.Pages.Contacts;


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

        var isAuthorized = User.IsInRole(Constants.ContactManagersRole) ||
            User.IsInRole(Constants.ContactAdminstratorsRole);

        var currentUserId = UserManager.GetUserId(User);

        // Only the owner and authorized users can see approved contacts
        if (isAuthorized)
        {
            contacts = contacts.Where(c => c.Status == ContactStatus.Approved || c.OwnerId == currentUserId);
        }

        Contact = await contacts.ToListAsync();
    }
}
