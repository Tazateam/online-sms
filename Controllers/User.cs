using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using online_sms.Models;

namespace online_sms.Controllers
{
	public class User : Controller
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
        public IActionResult Signup(User use)
        {
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
