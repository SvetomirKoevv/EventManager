using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVCEventManager.Areas.Identity.Pages.Account.Manage
{
    public class AdminPanelModel : PageModel
    {
        private EventContext eventContext;
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        public AdminPanelModel(EventContext eventContext_, UserManager<User> userManager_, SignInManager<User> signInManager_)
        {
            eventContext = eventContext_;
            userManager = userManager_;
            signInManager = signInManager_;
        }

        public List<Event> Events = new List<Event>();
        public async Task<IActionResult> OnGetAsync()
        {
            if (!await userManager.IsInRoleAsync(await signInManager.UserManager.GetUserAsync(User), Role.ADMINISTRATOR.ToString()))
            {
                return RedirectToPage("../AccessDenied");
            }
            Events = (List<Event>)await eventContext.ReadAllAsync(true);
            return Page();
        }
    }
}
