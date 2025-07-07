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
using MVCEventManager.Services;
using MVCEventManager.Models.OtherModels;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

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

        public async Task<IActionResult> Index(Dictionary<string, string> pars)
        {
            List<Event> events = (List<Event>)await context.ReadAllAsync(true);
            IndexEventModel model = new IndexEventModel
            {
                Events = events
            };
            if (pars.ContainsKey("SortOrder"))
            {
                model.SortOrder = pars["SortOrder"];
            }
            else
            {
                model.SortOrder = "Inc";
            }

            if (model.SortOrder == "Dec")
            {
                events = events.OrderByDescending(e => e.EventStart).ToList();
            }
            else
            {
                events = events.OrderBy(e => e.EventStart).ToList();
            }
            if (pars.ContainsKey("SearchQuery"))
            {
                model.SearchQuery = pars["SearchQuery"];
            }
            else
            {
                model.SearchQuery = "";
            }

            model.Events = model.Events.Where(x => x.Name.Contains(model.SearchQuery)).ToList();
            return View(model);
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

        [Authorize(Roles = "ADMINISTRATOR, USER")]
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

            if (dateTime < DateTime.Now.Date)
            {
                MessageModel newMessage = new MessageModel
                {
                    Message = $"Date Cannot be in the past!"
                };
                List<MessageModel> newMessages = TempData.Peek("UserMessages") != null ? JsonConvert.DeserializeObject<List<MessageModel>>(TempData.Peek("UserMessages") as string) : new List<MessageModel>();
                newMessages.Add(newMessage);

                TempData["UserMessages"] = JsonConvert.SerializeObject(newMessages);
                return RedirectToAction("Index", "Home");
            }

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
        [Authorize(Roles = "ADMINISTRATOR, USER")]
        public async Task<IActionResult> CreateTime(EventTimeCreateModel model)
        {
            TempData["CreateTimeTemp"] = JsonConvert.SerializeObject(model);

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = Url.Action("CreateTime", "Events") });

            }

            User loggedInUser = await userManager.FindByNameAsync(User.Identity.Name);
            model.Creator = loggedInUser;

            if (!model.IsDateAndTimeValid)
            {
                model.DateAndTimeValidatedText = "Time must be after the current time!";
                return View(model);
            }

            if (ModelState[nameof(EventTimeCreateModel.Name)].Errors.Count == 0 &&
                ModelState[nameof(EventTimeCreateModel.Description)].Errors.Count == 0 &&
                ModelState[nameof(EventTimeCreateModel.TimeOnly)].Errors.Count == 0 &&
                ModelState[nameof(EventTimeCreateModel.Location)].Errors.Count == 0)
            {
                DateTime eventDateTime = new DateTime(model.DateOnly.Year, 
                                                      model.DateOnly.Month, 
                                                      model.DateOnly.Day, 
                                                      model.TimeOnly.Hour, 
                                                      model.TimeOnly.Minute, 
                                                      0);
                Event newEvent = new Event()
                {
                    Name = model.Name,
                    Description = model.Description,
                    EventStart = eventDateTime,
                    Location = model.Location,
                    MaxAttendees = model.MaxAttendees
                };
                MessageModel newMessage = new MessageModel
                {
                    Message = $"New event scheduled for {newEvent.EventStart.ToString("yyyy/MM/dd - HH:mm")}"
                };
                List<MessageModel> newMessages = TempData.Peek("UserMessages") != null ? JsonConvert.DeserializeObject<List<MessageModel>>(TempData.Peek("UserMessages") as string) : new List<MessageModel>();
                newMessages.Add(newMessage);

                TempData["UserMessages"] = JsonConvert.SerializeObject(newMessages);
                TempData.Remove("CreateTimeTemp");

                await context.CreateAsync(newEvent);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Roles = "ADMINISTRATOR, USER")]
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
            @event.Creator = loggedInUser;
            
            if (ModelState[nameof(Event.EventStart)].Errors.Count > 0)
            {
                return View();
            }

            ModelState.ClearValidationState(nameof(Event.Creator));
            TryValidateModel(@event);

            if (ModelState.IsValid)
            {
                await context.CreateAsync(@event);
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        [Authorize(Roles = "ADMINISTRATOR, USER")]
        public async Task<IActionResult> Edit(int id, string returnUrl)
        {
            var @event = await context.ReadAsync(id, true);

            if (@event.EventStart < DateTime.Now)
            {
                MessageModel newMessage = new MessageModel
                {
                    Message = $"Cannot edit past events!"
                };
                List<MessageModel> newMessages = TempData.Peek("UserMessages") != null ? JsonConvert.DeserializeObject<List<MessageModel>>(TempData.Peek("UserMessages") as string) : new List<MessageModel>();
                newMessages.Add(newMessage);

                TempData["UserMessages"] = JsonConvert.SerializeObject(newMessages);
                return LocalRedirect(returnUrl);
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            User loggedInUser = await userManager.FindByNameAsync(User.Identity.Name);

            if (@event.Creator != loggedInUser)
            {
                return Unauthorized();
            }
            
            if (@event == null)
            {
                return NotFound();
            }
            
            ReturnEventModel editEventModel = new ReturnEventModel
            {
                Id = @event.Id,
                Name = @event.Name,
                Description = @event.Description,
                Location = @event.Location,
                MaxAttendees = @event.MaxAttendees,
                ReturnUrl = returnUrl
            };

            return View(editEventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR, USER")]
        public async Task<IActionResult> Edit(ReturnEventModel @event)
        {
            if (ModelState[nameof(Event.EventStart)].Errors.Count > 0)
            {
                return View();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            User loggedInUser = await userManager.FindByNameAsync(User.Identity.Name);
            Event eventFromDb = await context.ReadAsync(@event.Id, true);
            if (eventFromDb.Creator != loggedInUser)
            {
                return Unauthorized();
            }

            @event.Creator = loggedInUser;

            ModelState.ClearValidationState(nameof(Event.Creator));
            TryValidateModel(@event);

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
                return LocalRedirect(@event.ReturnUrl);
            }
            return View(@event);
        }

        public async Task<IActionResult> Delete(int id, string returnUrl)
        {

            Event @event = await context.ReadAsync(id);
            
            if (@event == null)
            {
                return NotFound();
            }

            ReturnEventModel deleteEventModel = new ReturnEventModel
            {
                Id = @event.Id,
                Name = @event.Name,
                Description = @event.Description,
                Location = @event.Location,
                MaxAttendees = @event.MaxAttendees,
                ReturnUrl = returnUrl
            };

            return View(deleteEventModel);  
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl)
        {
            await context.DeleteAsync(id);

            return LocalRedirect(returnUrl);
        }

        private bool EventExists(int id)
        {
            return context.ReadAllAsync().Result.Any(e => e.Id == id);
        }
    }
}
