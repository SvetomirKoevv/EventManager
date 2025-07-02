using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVCEventManager.Areas.Identity.Pages.Account.Manage
{
    public class AdminPanelModel : PageModel
    {
        private EventContext eventContext;
        public AdminPanelModel(EventContext eventContext_)
        {
            eventContext = eventContext_;
        }

        public List<Event> Events = new List<Event>();
        public async Task OnGetAsync()
        {
            Events = (List<Event>)await eventContext.ReadAllAsync(true);
        }
    }
}
