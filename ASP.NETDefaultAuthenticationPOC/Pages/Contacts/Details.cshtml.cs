using ASP.NETDefaultAuthenticationPOC.Data;
using ASP.NETDefaultAuthenticationPOC.Models;
using ASP.NETDefaultAuthenticationPOC.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETDefaultAuthenticationPOC.Pages.Contacts
{
    public class DetailsModel : BasePageModel
    {
        public DetailsModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
        {
        }


        public Contact Contact { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Contact? contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            Contact = contact;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Read);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized.Succeeded && currentUserId != Contact.OwnerId && Contact.Status != ContactStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id, ContactStatus status)
        {
            Contact? contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var contactOperation = status == ContactStatus.Approved ?
                ContactOperations.Approve : ContactOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact, contactOperation);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            contact.Status = status;
            Context.Contact.Update(contact);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }


    // This model allows anonymous users
    [AllowAnonymous]
    public class DetailsForAnonymousModel : BasePageModel
    {
        public DetailsForAnonymousModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }


        public Contact Contact { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            Contact? contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            Contact = contact;

            if (!User.Identity!.IsAuthenticated)
            {
                return Challenge();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Read);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized.Succeeded
                && currentUserId != Contact.OwnerId
                && Contact.Status != ContactStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }
    }
}
