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
using Microsoft.IdentityModel.Tokens;
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
        public IActionResult Profile(int id)
		{
            var data = db.Users.Where(u => u.UserId == id).FirstOrDefault();
			ViewBag.Username = data.Username;
			ViewBag.Image = data.ProfilePhoto;
			return View(data);
          

        }


		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public IActionResult Profile(User us, IFormFile ProfilePhoto)
		{
			var data = db.Users.Where(u => u.UserId == us.UserId).FirstOrDefault();

			if (data != null)
			{
				// Update other fields
				data.FirstName = us.FirstName;
				data.LastName = us.LastName;
				data.Gender = us.Gender;
				data.Dob = us.Dob;
				data.Address = us.Address;
				data.MaritalStatus = us.MaritalStatus;
				data.Qualification = us.Qualification;
				data.Sports = us.Sports;
				data.Hobbies = us.Hobbies;
				data.Designation = us.Designation;

				// Handle Profile Photo Upload
				if (ProfilePhoto != null && ProfilePhoto.Length > 0)
				{
					// Generate a unique filename using the user's ID
					var fileName = $"{us.UserId}_{Path.GetFileName(ProfilePhoto.FileName)}";
					var filePath = Path.Combine("wwwroot", "assets", "userimages", fileName);

					// Save the file to the server
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						ProfilePhoto.CopyTo(stream);
					}

					// Update the profile photo path in the database
					data.ProfilePhoto = $"/assets/userimages/{fileName}";
				}

				db.SaveChanges();
			}

			return RedirectToAction("Profile");
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
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));

            // Get the list of user IDs who are already friends with the current user
            var friendIds = db.Friends
                .Where(f => f.UserId == userId || f.FriendUserId == userId)
                .Select(f => f.UserId == userId ? f.FriendUserId : f.UserId)
                .ToList();

            // Get the list of users excluding the current user and the users who are already friends
            var contacts = db.Users
                .Where(u => u.UserId != userId && !friendIds.Contains(u.UserId))
                .ToList();

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
        public ActionResult GetFriendsAll()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
            var friends = db.Friends
                .Where(f =>
                    (f.UserId == userId || f.FriendUserId == userId) &&
                    f.Status == "Accepted"
                )
                .Select(f => f.UserId == userId ? f.FriendUser : f.User) // Select the friend user
                .Distinct() // Ensure unique friends are returned
                .ToList();

            return PartialView("_AllFriends", friends);
        }

        [HttpPost]
        public IActionResult RespondToFriendRequest(int friend_id, string response)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));

            if (response == "Reject" || response == "Unfriend")
            {
                // Find the friendship records in both directions
                var friendRequest = db.Friends
                    .FirstOrDefault(f => f.FriendUserId == friend_id&& f.UserId == userId);

                var reciprocalFriendship = db.Friends
                    .FirstOrDefault(f => f.UserId == userId && f.FriendUserId == friend_id);

                if (friendRequest != null)
                {
                    db.Friends.Remove(friendRequest);
                }

                if (reciprocalFriendship != null)
                {
                    db.Friends.Remove(reciprocalFriendship);
                }

                db.SaveChanges();
            }
            else if (response == "Accept")
            {
                // Accept friend logic (already implemented)
                var friendRequest = db.Friends
                    .FirstOrDefault(f => f.FriendUserId == userId && f.UserId == friend_id && f.Status == "Pending");

                if (friendRequest != null)
                {
                    friendRequest.Status = "Accepted";

                    var reciprocalFriendship = db.Friends
                        .FirstOrDefault(f => f.UserId == friend_id && f.FriendUserId == userId);

                    if (reciprocalFriendship == null)
                    {
                        db.Friends.Add(new Friend { UserId = friend_id, FriendUserId = userId, Status = "Accepted" });
                    }

                    db.SaveChanges();
                }
            }

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

        private readonly string MyUsername = "923422704726"; // Your SendPK username
        private readonly string MyPassword = "Merijaan"; // Your SendPK password
        private readonly string Masking = "SMS Alert"; // Your Company Brand Name
      
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

		//public IActionResult sendBulkMessage(string reciverNumber, string message)
		//{
		//    string result = sendSMS(reciverNumber, message);
		//    ViewBag.Message = result; // Display the response from the API
		//    return View();
		//}

		//public string sendSMS(string reciver, string message)
		//{
		//    String msg = HttpUtility.UrlEncode(message);
		//    using (var wb = new WebClient())
		//    {
		//        byte[] response = wb.UploadValues("https://api.txtlocal.com/send/", new NameValueCollection()
		//        {
		//        {"apikey" , "MzI2NzU2NzE0NDY5NTY1YTQzNjI2MTZmNmE3YTM3NzE="},
		//        {"numbers" , "+923422704726"},
		//        {"message" , msg},
		//        {"sender" , "Zeeshan"}
		//        });
		//        string result = System.Text.Encoding.UTF8.GetString(response);
		//        return result;
		//    }
		//}


		private string MyApiKey = "73efec4a491d801bc7eb723832cf02f9";
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SendBulkMessage(string receiverNumber, string message)
		{
			string result = SendSMS(MyApiKey, receiverNumber, "Default", message);
			ViewBag.Message = result; // Display the response from the API
			return View();
		}

		public string SendSMS(string apiKey, string receiver, string sender, string textMessage)
		{
			// API endpoint for sending SMS
			string uri = "https://api.veevotech.com/v3/sendsms";

			// Prepare the request parameters
			string postData = $"hash={apiKey}&receivernum={receiver}&medium=1&sendernum={sender}&text_message={Uri.EscapeDataString(textMessage)}";

			try
			{
				// Create the web request
				WebRequest request = WebRequest.Create(uri);
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";

				// Write the POST data to the request
				byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
				request.ContentLength = byteArray.Length;

				using (Stream dataStream = request.GetRequestStream())
				{
					dataStream.Write(byteArray, 0, byteArray.Length);
				}

				// Get the response from the API
				using (WebResponse response = request.GetResponse())
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(responseStream))
						{
							return reader.ReadToEnd().Trim();
						}
					}
				}
			}
			catch (WebException ex)
			{
				// Handle specific HTTP error responses
				if (ex.Response is HttpWebResponse httpWebResponse)
				{
					switch (httpWebResponse.StatusCode)
					{
						case HttpStatusCode.NotFound:
							return "404: URL not found: " + uri;
						case HttpStatusCode.BadRequest:
							return "400: Bad Request - Please check your parameters.";
						default:
							return httpWebResponse.StatusCode.ToString();
					}
				}
				return "Error occurred while sending SMS. Details: " + ex.Message;
			}
		}




	}
}
