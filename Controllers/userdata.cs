using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using online_sms.Models;
using System.Security.Claims;

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
				return RedirectToAction("Login", "userdata");

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
			var currentUserId = User.FindFirstValue(ClaimTypes.Sid);

			var user = db.Users.FirstOrDefault(u => u.UserId == Convert.ToInt32(currentUserId));

			if (user != null)
			{
				ViewBag.UserName = user.Username;
				ViewBag.ProfilePhoto = user.ProfilePhoto; 
				ViewBag.Password = user.Password;
				ViewBag.Email = user.Email;
				ViewBag.MobileNumber = user.MobileNumber;
				ViewBag.FirstName = user.FirstName;
				ViewBag.LastName = user.LastName;
				ViewBag.Gender = user.Gender;
				ViewBag.Dob = user.Dob;
				ViewBag.Address = user.Address;
				ViewBag.MaritalStatus = user.MaritalStatus;
				ViewBag.Hobbies = user.Hobbies; 
                ViewBag.Sports = user.Sports;
				ViewBag.Qualification = user.Qualification;
				ViewBag.Designation = user.Designation;
			}
			else
			{
				ViewBag.UserName = "Profile not found.";
			}

			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Profile(User model)
		{
			if (ModelState.IsValid)
			{
				var currentUserId = User.FindFirstValue(ClaimTypes.Sid);
				var user = db.Users.FirstOrDefault(u => u.UserId == Convert.ToInt32(currentUserId));

				if (user == null)
				{
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
					user.ProfilePhoto = model.ProfilePhoto;
					ViewBag.Email = user.Email;
					user.Gender = model.Gender;
					user.Dob = model.Dob;
					user.Address = model.Address;
					user.MaritalStatus = model.MaritalStatus;
					user.Qualification = model.Qualification;
					user.Sports = model.Sports;
					user.Hobbies = model.Hobbies;
					user.Designation = model.Designation;

					db.Users.Update(user);
					db.SaveChanges();

					TempData["SuccessMessage"] = "Profile updated successfully!";
					return RedirectToAction("Profile");
				}
				else
				{
					ViewBag.ErrorMessage = "Profile not found.";
					return View("Error");
				}
			}
			return View(model);
		}
	

	public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User logg)
        {
            var user = db.Users.FirstOrDefault(x => x.Email == logg.Email && x.Password == logg.Password);

            if (user != null)
            {
                var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Email, logg.Email),
			new Claim(ClaimTypes.Name, user.Username),
			new Claim(ClaimTypes.Sid, user.UserId.ToString())
        }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "userdata");
            }

            return View();
        }
        public IActionResult Logout()
        {
            var lgoin = HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

		
		public IActionResult Add_Contact()
        {
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult Add_Contact(Contact con)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    ModelState.AddModelError("", "User is not authenticated.");
                    return View(con); 
                }

                int userIdInt;
                if (!int.TryParse(userId, out userIdInt))
                {
                    ModelState.AddModelError("", "Invalid User ID.");
                    return View(con);
                }
                var contact = new Contact
                {
                    FirstName = con.FirstName,
                    LastName = con.LastName,
                    ContactNumber = con.ContactNumber,
                    UserId = userIdInt 
                };

                db.Contacts.Add(con);
                db.SaveChanges();
            }
            return View(con);
        }

        public IActionResult Inbox()
        {
            return View();
        }
      
    }
}
