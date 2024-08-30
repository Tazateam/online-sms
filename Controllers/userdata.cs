using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using online_sms.Models;
using System.Security.Claims;
using System.Net;
using System.Web;
using RestSharp;
using System.Threading.Tasks;
using Infobip.Api.SDK;
using Infobip.Api.SDK.SMS.Models;

using System.Collections.Specialized;
namespace online_sms.Controllers
{
    public class userdata : Controller
    {
        OnlineMessagesContext db = new OnlineMessagesContext();

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Signup()
        {
            ViewBag.a = new SelectList(db.Users, "UserId", "Username");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
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
        [Authorize]
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
        [Authorize]
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
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
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
        [Authorize]
        public IActionResult Logout()
        {
            var lgoin = HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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


        //private readonly string MyApiKey = "923422704726-6b2eb1f1-1af6-4c6b-b016-7cf09f8cd3ae"; // Your API Key
        //private readonly string MyUsername = "923422704726"; // Your SendPK username
        //private readonly string MyPassword = "Merijaan"; // Your SendPK password
        //private readonly string Masking = "SMS Alert"; // Your Company Brand Name
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> sendBulkMessage(string reciverNumber, string message)
        //{
        //    var configuration = new ApiClientConfiguration(
        //        "https://e1vz92.api.infobip.com",
        //        "f23377a18161d10f311f63d1defe8a7d-6c5dce47-5a02-4604-961a-c7d0ef4c6cd2"
        //    );
        //    var client = new InfobipApiClient(configuration);

        //    var destination = new SmsDestination(
        //        to:"923132239840"
        //    );
        //    var msg = new SmsMessage(
        //        destinations: new List<SmsDestination> { destination },
        //        from: "Infobip SMS",
        //        text: message
        //    );
        //    var request = new SendSmsMessageRequest(
        //        messages: new List<SmsMessage> { msg }
        //    );

        //    var response = await client.Sms.SendSmsMessage(request);

        //    ViewBag.Message = response?.Messages?[0]?.Status?.Description ?? "SMS sent successfully!";
        //    return View();
        //}

        public IActionResult sendBulkMessage(string reciverNumber, string message)
        {
            string result = sendSMS(reciverNumber, message);
            ViewBag.Message = result; // Display the response from the API
            return View();
        }

        public string sendSMS(string reciver, string message)
        {
            String msg = HttpUtility.UrlEncode(message);
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues("https://api.txtlocal.com/send/", new NameValueCollection()
                {
                {"apikey" , "MzI2NzU2NzE0NDY5NTY1YTQzNjI2MTZmNmE3YTM3NzE="},
                {"numbers" , "+923422704726"},
                {"message" , msg},
                {"sender" , "Zeeshan"}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
        }

        //public IActionResult sendBulkMessage(string reciverNumber, string message)
        //{
        //    string result = sendSMS(reciverNumber, message);
        //    ViewBag.Message = result; // Display the response from the API
        //    return View();
        //}

        //public string sendSMS(string reciverNumber, string message)
        //{
        //    string URI = "https://sendpk.com/api/sms.php?" +
        //                 "api_key=" + MyApiKey +
        //                 "&sender=" + Masking +
        //                 "&mobile=" + 923142780007 +
        //                 "&message=" + Uri.UnescapeDataString(message);

        //    try
        //    {
        //        WebRequest req = WebRequest.Create(URI);
        //        WebResponse resp = req.GetResponse();
        //        using (var sr = new System.IO.StreamReader(resp.GetResponseStream()))
        //        {
        //            return sr.ReadToEnd().Trim();
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        var httpWebResponse = ex.Response as HttpWebResponse;
        //        if (httpWebResponse != null)
        //        {
        //            switch (httpWebResponse.StatusCode)
        //            {
        //                case HttpStatusCode.NotFound:
        //                    return "404: URL not found: " + URI;
        //                case HttpStatusCode.BadRequest:
        //                    return "400: Bad Request";
        //                default:
        //                    return httpWebResponse.StatusCode.ToString();
        //            }
        //        }
        //        return "Error occurred while sending SMS.";
        //    }
        //}



    }
}
