using ASP.NETDefaultAuthenticationPOC.Authorization;
using ASP.NETDefaultAuthenticationPOC.Data;
using ASP.NETDefaultAuthenticationPOC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETDefaultAuthenticationPOC.Pages.Contacts
{
    public class EditModel : BasePageModel
    {
        public EditModel(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        UserManager<IdentityUser> userManager)
        : base(context, authorizationService, userManager)
        {
        }


        [BindProperty]
        public Contact Contact { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Contact? contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            Contact = contact;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Context, ContactOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch Contact for the OwnerId
            var contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == Contact.ContactId);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Context, ContactOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Contact.OwnerId = contact.OwnerId;

            Context.Attach(Contact).State = EntityState.Modified;

            if (Contact.Status == ContactStatus.Approved)
            {
                // If the contact is updated after approval, and the user can't approve it
                // then set it to submitted so the update can be checked and approved
                var canApprove = await AuthorizationService.AuthorizeAsync(User, Context, ContactOperations.Update);

                if (!canApprove.Succeeded)
                {
                    Contact.Status = ContactStatus.Submitted;
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
