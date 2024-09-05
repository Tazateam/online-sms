using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_sms.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
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
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,Email,MobileNumber,VerificationCode,IsVerified,CreatedAt,FirstName,LastName,Gender,Dob,Address,MaritalStatus,Hobbies,Sports,ProfilePhoto,Qualification,Designation")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || db.Users == null)
            {
                return NotFound();
            }

            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,Email,MobileNumber,VerificationCode,IsVerified,CreatedAt,FirstName,LastName,Gender,Dob,Address,MaritalStatus,Hobbies,Sports,ProfilePhoto,Qualification,Designation")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(user);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || db.Users == null)
            {
                return NotFound();
            }

            var user = await db.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (db.Users == null)
            {
                return Problem("Entity set 'OnlineMessagesContext.Users'  is null.");
            }
            var user = await db.Users.FindAsync(id);
            if (user != null)
            {
                db.Users.Remove(user);
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (db.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
        [AllowAnonymous]
        public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
        [AllowAnonymous]
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
        [Authorize]
        public IActionResult Logout()
		{
			var lgoin = HttpContext.SignOutAsync
				(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("login", "Admin");
		}
        public async Task<IActionResult> PackageRequests()
        {
            // Fetch all package requests from the database
            var requests = await db.PackageRequests
                .Include(r => r.User) // Include related User data if needed
                .ToListAsync();

            // Pass the requests to the view
            return View(requests);
        }

        public async Task<IActionResult> ApproveRequest(int id)
        {
            // Find the package request by ID
            var request = await db.PackageRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound(); // Handle case where request is not found
            }

            // Find the user associated with the request
            var user = await db.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(); // Handle case where user is not found
            }

            // Update user's message count based on package type
            if (request.PackageType == "Silver")
            {
                user.MsgCount = 50;
            }
            else if (request.PackageType == "Premium")
            {
                user.MsgCount = 100;
            }
            else
            {
                return BadRequest("Invalid package type"); // Handle unexpected package type
            }

            // Update user package type and status
            user.PackageType = request.PackageType;
            db.Users.Update(user);

            // Update request status
            request.Status = "Approved";
            db.PackageRequests.Update(request);

            // Save changes to the database
            await db.SaveChangesAsync();

            // Redirect to the list of package requests
            return RedirectToAction("PackageRequests");
        }


    }
}
    