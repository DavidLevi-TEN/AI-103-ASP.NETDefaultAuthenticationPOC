using ContactManager.Data;
using ContactManager.Models;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Pages.Contacts
{
    public class DeleteModel : BasePageModel
    {
        public DeleteModel(
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

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Context, ContactOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Contact? contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Context, ContactOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Contact.Remove(contact);
            await Context.Contact.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
