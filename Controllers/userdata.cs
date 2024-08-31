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

        public ActionResult GetSuggested()
        {
            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.Sid);

            // Get the list of users excluding the current user
            var contacts = db.Users.Where(u => u.UserId != Convert.ToInt32(userId)).ToList();

            // Return the filtered list of users to the partial view
            return PartialView("_SuggestedUser", contacts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFriendRequest(Friend frnd,int friend_id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.Sid);
            var contact = new Friend
            {
                UserId = Convert.ToInt32(currentUserId),
                FriendUserId= friend_id,
                Status = "Pending",
              
            };
            db.Friends.Add(contact); 
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult GetFriendRequest()
        {
            
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));

            // Get the list of friend requests where the current user is the FriendUserId (receiving the request)
            var friendRequests = db.Friends
                .Where(f => f.FriendUserId == userId && f.Status == "Pending")
                .Select(f => f.User) // Select the User who sent the request
                .ToList();

            // Return the list of users who sent the friend requests to the partial view
            return PartialView("_GetFriendRequest", friendRequests);
        }

        public ActionResult GetFriends()
        {

            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));

            // Get the list of friends where the current user is either UserId or FriendUserId and the status is "Accepted"
            var friends = db.Friends
                .Where(f =>
                    (f.UserId == userId || f.FriendUserId == userId) &&
                    f.Status == "Accepted"
                )
                .Select(f => f.UserId == userId ? f.FriendUser : f.User) // Select the friend user
                .Distinct() // Ensure unique friends are returned
                .ToList();

            return PartialView("_Friends", friends);
        }


        [HttpPost]
        public IActionResult RespondToFriendRequest(int friend_id, string response)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));

            // Find the existing friend request
            var friendRequest = db.Friends
                .FirstOrDefault(f => f.FriendUserId == userId && f.UserId == friend_id && f.Status == "Pending");

            if (friendRequest != null)
            {
                if (response == "Accept")
                {
                    // Update the status of the friend request to "Accepted"
                    friendRequest.Status = "Accepted";

                    // Check if the reciprocal friendship exists; if not, create it
                    var reciprocalFriendship = db.Friends
                        .FirstOrDefault(f => f.UserId == friend_id && f.FriendUserId == userId);

                    if (reciprocalFriendship == null)
                    {
                        db.Friends.Add(new Friend { UserId = friend_id, FriendUserId = userId, Status = "Accepted" });
                    }
                }
                else if (response == "Reject")
                {
                    // Update the status of the friend request to "Rejected"
                    friendRequest.Status = "Rejected";
                }

                db.SaveChanges();
            }

            // Redirect to the same view or another appropriate action
            return RedirectToAction("Index");
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
