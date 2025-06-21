using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCEventManager.Models;
using MVCEventManager.Models.OtherModels;
using MVCEventManager.Services;
using Newtonsoft.Json;
using NuGet.Protocol.Resources;

namespace MVCEventManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult _AlertsPartial(string messagesJson)
    {
        if (TempData.Peek("UserMessages") != null)
        {
            List<MessageModel> messages = JsonConvert.DeserializeObject<List<MessageModel>>(TempData.Peek("UserMessages") as string);
            List<MessageModel> unexpiredMessages = new List<MessageModel>();
            foreach (MessageModel message in messages)
            {
                if (message.Expires < DateTime.Now)
                {
                    continue;
                }
                unexpiredMessages.Add(message);
            }
            TempData["UserMessages"] = JsonConvert.SerializeObject(unexpiredMessages);
            return PartialView("_AlertsPartial", unexpiredMessages);
        }
        else
        {
            return PartialView("_AlertsPartial", new List<MessageModel>());
        }
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
