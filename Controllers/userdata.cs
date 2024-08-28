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

			var email = User.Identity.Name;

			if (string.IsNullOrEmpty(email))
			{
				ViewBag.ErrorMessage = "User email not found.";
				return View();
			}

			var profile = db.UserProfiles.FirstOrDefault(up => up.Email == email);

			if (profile == null)
			{
				ViewBag.ErrorMessage = "Profile not found.";
				return View();
			}

			var model = new UserProfile
			{
				Name = profile.Name,
				Gender = profile.Gender,
				Dob = profile.Dob,
				Address = profile.Address,
				MaritalStatus = profile.MaritalStatus,
				Qualification = profile.Qualification,
				Sports = profile.Sports,
				Hobbies = profile.Hobbies,
				Designation = profile.Designation,
				Email = profile.Email,
				ProfilePhoto = profile.ProfilePhoto
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Profile(UserProfile model)
		{
			if (ModelState.IsValid)
			{
				var profile = db.UserProfiles.FirstOrDefault(up => up.Email == model.Email);

				if (profile == null)
				{
					ViewBag.ErrorMessage = "Profile not found.";
					return View(model);
				}
				profile.Name = model.Name;
				profile.Gender = model.Gender;
				profile.Dob = model.Dob;
				profile.Address = model.Address;
				profile.MaritalStatus = model.MaritalStatus;
				profile.Qualification = model.Qualification;
				profile.Sports = model.Sports;
				profile.Hobbies = model.Hobbies;
				profile.Designation = model.Designation;
				profile.Email = model.Email;
				profile.ProfilePhoto = model.ProfilePhoto;

				db.UserProfiles.Add(model);
				db.SaveChanges();

				TempData["SuccessMessage"] = "Profile updated successfully!";
				return RedirectToAction("Profile");
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
