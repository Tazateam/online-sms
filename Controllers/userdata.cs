using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using online_sms.Models;
using System.Security.Claims;
using System.Collections.Specialized;
using System.Net;
using System.Web;
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





        public IActionResult Inbox()
        {
            // Get the current user's ID (you might need to adjust this based on your authentication setup)
            var currentUserId = User.FindFirstValue(ClaimTypes.Sid); // or use another method to get the current user's ID

            // Fetch users from the database
            var users = db.Users.ToList();

            // Exclude the current user from the list
            var filteredUsers = users.Where(u => u.UserId != Convert.ToInt32(currentUserId)).ToList();

            // Pass the filtered users list to the view using ViewBag
            ViewBag.Users = filteredUsers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Inbox(Message model, int? SenderUserId, int? ReceiverUserId)
        {
            if (ModelState.IsValid)
            {
                // Save the new message to the database
                var message = new Message
                {
                    SenderUserId = SenderUserId,
                    ReceiverUserId = ReceiverUserId,
                    MessageText = model.MessageText,
                    SentAt = DateTime.Now
                };

                db.Messages.Add(message);
                db.SaveChanges();

                // Return the updated chat history as JSON
                var chatMessages = db.Messages
                    .Where(m => (m.SenderUserId == SenderUserId && m.ReceiverUserId == ReceiverUserId) ||
                                (m.SenderUserId == ReceiverUserId && m.ReceiverUserId == SenderUserId))
                    .ToList();

                return Json(new { success = true, messages = chatMessages });
            }

            return Json(new { success = false, message = "Failed to send message" });
        }


        [HttpGet]
        public IActionResult GetMessages(int receiverId)
        {
            var userIdString = User.FindFirst(ClaimTypes.Sid)?.Value;

            try
            {
                int userId = Convert.ToInt32(userIdString);
                var messages = GetMessages(userId, receiverId);
                return Json(new { success = true, messages });
            }
            catch (FormatException)
            {
                // Handle the case where conversion fails
                return Json(new { success = false, message = "Invalid user ID format" });
            }
            catch (OverflowException)
            {
                // Handle the case where the number is too large or too small
                return Json(new { success = false, message = "User ID is out of range" });
            }
        }

        public IEnumerable<Message> GetMessages(int userId, int receiverId)
        {
            // Example using Entity Framework
            return db.Messages
                     .Where(m => (m.SenderUserId == userId && m.ReceiverUserId == receiverId) ||
                                 (m.SenderUserId == receiverId && m.ReceiverUserId == userId))
                     .ToList();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Contact con)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.Sid);

                if (userId == null)
                {
                    ModelState.AddModelError("", "User is not authenticated.");
                    return View(con);  // This will try to return the current view with errors.
                }

                if (!int.TryParse(userId, out int userIdInt))
                {
                    ModelState.AddModelError("", "Invalid User ID.");
                    return View(con);  // This will try to return the current view with errors.
                }
                var contact = new Contact
                {
                    FirstName = con?.FirstName?.Trim(),
                    LastName = con?.LastName?.Trim(),
                    ContactNumber = con?.ContactNumber?.Trim(), 
                    UserId = userIdInt
                };

                db.Contacts.Add(contact);  // Ensure you add the newly created contact, not the form's con object
                db.SaveChanges();

                // If you want to redirect after successfully adding the contact
                return RedirectToAction("Index");  // Redirect to Index action after adding
            }

            return View(con);  // Return the current view with the model (including validation errors).
        }

        public ActionResult GetContacts()
        {
            var contacts = db.Contacts.ToList(); 
            return PartialView("_ContactsPartial", contacts); 
        }

        public ActionResult SendBulkMessage(int contactId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.Sid);

            // Get the contact that matches the contactId
            var contact = db.Contacts.SingleOrDefault(c => c.ContactId == contactId);

            if (contact == null)
            {
                // Handle the case where the contact is not found
                return NotFound();
            }

            // Send the contact's phone number to the view using ViewBag
            ViewBag.ContactNumber = contact.ContactNumber;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult sendBulkMessage(string reciverNumber, string message)
        {
            string result = sendSMS(reciverNumber, message);
            ViewBag.Message = "SMS sent successfully!";
            return View(); 
        }
        public string sendSMS(string reciverNumber, string message)
        {
            String encodedMessage = HttpUtility.UrlEncode(message);
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.txtlocal.com/send/", new NameValueCollection()
            {
                {"apikey" , "MzI2NzU2NzE0NDY5NTY1YTQzNjI2MTZmNmE3YTM3NzE="}, 
                {"numbers" , reciverNumber},
                {"message" , encodedMessage},
                {"sender" , "Zeeshan "}
            });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }







    }
}
