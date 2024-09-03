using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_sms.Models;

namespace online_sms.Controllers
{
    public class AdminController : Controller
    {
        OnlineMessagesContext db = new OnlineMessagesContext();
        public async Task<IActionResult> Index()
        {
            var users = await db.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Contact()
        {
            var contacts = await db.Contacts.Include(c => c.User).ToListAsync();
            return View(contacts);
        }
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(Admin adm)
		{
			if (ModelState.IsValid)
			{
				
				var admin = await db.Admins
					.SingleOrDefaultAsync(a => a.Email == adm.Email);

				if (admin != null && admin.Password == adm.Password) 
				{
					
					return RedirectToAction("Index", "Admin"); 
				}
				else
				{
					ModelState.AddModelError("", "Invalid email or password.");
				}
			}

			
			return View(adm);
		}
	}
}
    