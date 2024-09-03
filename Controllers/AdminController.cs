using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_sms.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
namespace online_sms.Controllers
{
    public class AdminController : Controller
    {
        OnlineMessagesContext db = new OnlineMessagesContext();
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await db.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
    