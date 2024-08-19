using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using online_sms.Models;

namespace online_sms.Controllers
{
    public class userdata : Controller
    {
        OnlineMessagesContext db = new OnlineMessagesContext(); 
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Signup()
        {
            ViewBag.a = new SelectList(db.Users, "UserId", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(User enter)
        {
            var emails = db.Users.FirstOrDefault(Users => Users.Email == enter.Email);

            if (emails == null)
            {
                db.Users.Add(enter);
                db.SaveChanges();
            }
            else
            {
                ViewBag.b = "Already Registered";
            }
            ViewBag.a = new SelectList(db.Users, "UserId", "Username");
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

    }
}
