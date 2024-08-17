using Microsoft.AspNetCore.Mvc;

namespace online_sms.Controllers
{
	public class User : Controller
	{
        public IActionResult Index()
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
        public IActionResult Signup()
        {
            return View();
        }

    }
}
