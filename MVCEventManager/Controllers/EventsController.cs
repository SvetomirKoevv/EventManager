using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MVCEventManager.Models.EventModels;
using Newtonsoft.Json;
using Microsoft.SqlServer.Server;
using Microsoft.Identity.Client;
using Microsoft.Build.Framework;

namespace MVCEventManager.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventContext context;
        private readonly UserManager<User> userManager;

        public EventsController(EventContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.ReadAllAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            Event @event = await context.ReadAsync(id);
            
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        //[Authorize(Roles = "ADMINISTRATOR, USER")]
        public async Task<IActionResult> CreateTime(Dictionary<string, string> dateHolder)
        {

            if (dateHolder.Count == 0)
            {
                dateHolder = JsonConvert.DeserializeObject<Dictionary<string, string>>(TempData["Date-Data"] as string);
            }
            else
            {
                TempData["Date-Data"] = JsonConvert.SerializeObject(dateHolder);
            }

            int y = int.Parse(dateHolder["Year"]);
            int m = int.Parse(dateHolder["Month"]);
            int d = int.Parse(dateHolder["Day"]);

            DateTime dateTime = new DateTime(y, m, d);

            EventTimeCreateModel newModel = new EventTimeCreateModel()
            {
                DateOnly = dateTime,
            };
            if (TempData["CreateTimeTemp"] != null)
            {
                newModel = JsonConvert.DeserializeObject<EventTimeCreateModel>(TempData["CreateTimeTemp"] as string);
            }
            return View(newModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTime(EventTimeCreateModel model)
        {
            TempData["CreateTimeTemp"] = JsonConvert.SerializeObject(model);

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = Url.Action("CreateTime", "Events") });

            }

            User loggedInUser = await userManager.FindByNameAsync(User.Identity.Name);

            ModelState.Clear();
            model.Creator = loggedInUser;

            if (ModelState.IsValid)
            {
                DateTime eventDateTime = new DateTime(model.DateOnly.Year, 
                                                      model.DateOnly.Month, 
                                                      model.DateOnly.Day, 
                                                      model.TimeOnly.Hour, 
                                                      model.TimeOnly.Minute, 
                                                      1);
                Event newEvent = new Event()
                {
                    Name = model.Name,
                    Description = model.Description,
                    EventStart = eventDateTime,
                    Location = model.Location,
                    MaxAttendees = model.MaxAttendees
                };

                await context.CreateAsync(newEvent);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR, USER")]
        public async Task<IActionResult> Create(Event @event)
        {
            User loggedInUser = await userManager.FindByNameAsync(User.Identity.Name);
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            ModelState.Clear();
            @event.Creator = loggedInUser;

            if (ModelState.IsValid)
            {
                await context.CreateAsync(@event);
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        public async Task<IActionResult> Edit(int id)
        {

            var @event = await context.ReadAsync(id);
            
            if (@event == null)
            {
                return NotFound();
            }
            
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event @event)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    await context.UpdateAsync(@event);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        public async Task<IActionResult> Delete(int id)
        {

            var @event = await context.ReadAsync(id);
            
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await context.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return context.ReadAllAsync().Result.Any(e => e.Id == id);
        }
    }
}
