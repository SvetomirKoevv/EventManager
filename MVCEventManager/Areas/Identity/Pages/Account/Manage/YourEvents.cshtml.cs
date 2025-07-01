using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVCEventManager.Areas.Identity.Pages.Account.Manage
{
    public class YourEventsModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IdentityContext _identityContext;
        private User loggedInUser;

        public List<Event> Events = new List<Event>();
        public YourEventsModel(UserManager<User> userManager, SignInManager<User> signInManager, IdentityContext identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityContext = identityContext;
        }


        public async Task OnGet()
        {
            loggedInUser = await _identityContext.ReadUserAsync(_userManager.FindByNameAsync(User.Identity.Name).Result.Id, true);

            Events = loggedInUser.Events;
        }
    }
}
