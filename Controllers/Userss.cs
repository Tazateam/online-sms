using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using online_sms.Models;

namespace online_sms.Controllers
{
	public class Userss : Controller
	{
        OnlineMessagesContext db = new OnlineMessagesContext();
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Signup()
        {
            ViewBag.a = new SelectList(db.Users, "UserId", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(Userss euse)
        {
            var emails = db.Users.FirstOrDefault(Users => Users.Email == euse.email);
            //if (emails == null)
            //{
            //    db.Users.Add(use);
            //    db.SaveChanges();
            //}
            //else
            //{
            //    ViewBag.b = "Already Registered";
            //}
            ViewBag.a = new SelectList(db.Users, "UserId", "Username");
            return View();
        }
        
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
