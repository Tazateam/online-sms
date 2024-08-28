using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using online_sms.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace online_sms.Controllers
{
    public class HomeController : Controller
    {
        OnlineMessagesContext db = new OnlineMessagesContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactU conn)
        {
            if (ModelState.IsValid)
            {
                db.ContactUs.Add(conn);
                db.SaveChanges();
               
            }

            // If model state is not valid, return the same view with the model to show validation errors
            return View(conn);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}