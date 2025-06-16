using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Mvc;
using MVCEventManager.Models;
using System.Security;

namespace MVCEventManager.Controllers
{
    public class CallendarController : Controller
    {
        private readonly EventContext context;
        private readonly string[] months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        public CallendarController(EventContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> Index(Dictionary<string, string> data)
        {
            if (data.Count != 3)
            {
                data = new Dictionary<string, string>();
                data["Year"] = DateTime.Now.Year.ToString();
                data["Month"] = months[DateTime.Now.Month - 1];
                data["MonthIndex"] = DateTime.Now.Month.ToString();
            }
            else
            {
                data["Month"] = months[int.Parse(data["MonthIndex"]) - 1];
            }
            return View(data);
        }

        public async Task<IActionResult> Date(Dictionary<string, int> data)
        {
            int selectedYear = data["Year"];
            int selectedMonth = data["MonthIndex"];
            int selectedDay = data["Day"];

            List<Event> allEvents = context.ReadAllAsync()
                                        .Result
                                        .Where(x => x.EventStart.Year == selectedYear)
                                        .Where(x => x.EventStart.Month == selectedMonth)
                                        .Where(x => x.EventStart.Day == selectedDay)
                                        .ToList();

            DateViewModel viewModel = new DateViewModel
            {
                Events = allEvents,
                Year = selectedYear,
                MonthIndex = selectedMonth,
                Month = months[selectedMonth],
                Day = selectedDay
            };

            return View(viewModel);
        }
    }
}
