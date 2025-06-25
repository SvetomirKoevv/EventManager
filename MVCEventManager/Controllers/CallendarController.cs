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
        public static readonly string[] Months = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

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
                data["Month"] = Months[DateTime.Now.Month - 1];
                data["MonthIndex"] = DateTime.Now.Month.ToString();
            }
            else
            {
                data["Month"] = Months[int.Parse(data["MonthIndex"]) - 1];
            }
            return View(data);
        }

        public async Task<IActionResult> Date(Dictionary<string, string> data)
        {
            int selectedYear = int.Parse(data["Year"]);
            int selectedMonth = int.Parse(data["MonthIndex"]);
            int selectedDay = int.Parse(data["Day"]);

            List<Event> allEvents = context.ReadAllAsync()
                                        .Result
                                        .Where(x => x.EventStart.Year == selectedYear)
                                        .Where(x => x.EventStart.Month == selectedMonth)
                                        .Where(x => x.EventStart.Day == selectedDay)
                                        .ToList();

            if (data["SortType"] == "Name")
            {
                allEvents = allEvents.OrderBy(x => x.Name).ToList();
            }
            else if(data["SortType"] == "Location")
            {
                allEvents = allEvents.OrderBy(x => x.Location).ToList();
            }
            else if (data["SortType"] == "Time")
            {
                allEvents = allEvents.OrderBy(x => x.EventStart).ToList();
            }

            if (data["SortOrder"] == "Dec") allEvents.Reverse();

            DateViewModel viewModel = new DateViewModel
                {
                    Events = allEvents,
                    Year = selectedYear,
                    MonthIndex = selectedMonth,
                    Month = Months[selectedMonth - 1],
                    Day = selectedDay,
                    SortType = data["SortType"],
                    SortOrder = data["SortOrder"]
                };

            return View(viewModel);
        }
    }
}
