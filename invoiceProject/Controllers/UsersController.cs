using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using invoiceProject.Data;
using invoiceProject.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace invoiceProject.Controllers
{
    public class UsersController : Controller
    {
        private readonly invoiceProjectContext _context;
        public static int tempUserId;
        public UsersController(invoiceProjectContext context)
        {
            _context = context;
        }
        //----------------------------------------------------Index----------------------------------------------------
        // GET
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }
        //----------------------------------------------------Login----------------------------------------------------
        // GET
        public IActionResult Login()
        {
            return View();
        }
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(String UserName, String Password)
         {
            var user = _context.User.Where(u => u.UserName == UserName && u.Password == Password);
           
            if (user!=null && user.Count()>0)
            {
                tempUserId = user.FirstOrDefault().UserID;
                if (user.FirstOrDefault().IsAdmin)
                {
                    SignIn(user.First());
                    return RedirectPreserveMethod(nameof(Admin));
                }
                SignIn(user.First());
                return RedirectToAction(nameof(MyAccount));
            }
             return View();
         }
        //----------------------------------------------------Logout----------------------------------------------------
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        //----------------------------------------------------Session----------------------------------------------------
        private async void SignIn(User user)
        {
            //HttpContext.Session.SetString("Logged","1");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName),
                new Claim("FullName",user.FirstName+" " + user.LastName),
                new Claim(ClaimTypes.Role, user.IsAdmin.ToString()),
            };

            var ClaimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties { /*ExpiresUtc=DateTimeOffset.UtcNow.AddMinutes(10)*/};

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(ClaimsIdentity),
                authProperties);
        }
        //----------------------------------------------------Register----------------------------------------------------

        // GET
        public IActionResult Register()
        {
            return View();
        }

        // POST
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserID,FirstName,LastName,UserName,Password,IsAdmin,Email,EnteranceDate")] User user)
        {
            if (ModelState.IsValid && !Exists(user.UserName,user.Password))
            {
                user.EnteranceDate = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                tempUserId = user.UserID;
                SignIn(user);
                return RedirectToAction(nameof(MyAccount));
            }
            return View();
        }
        //----------------------------------------------------MyAccount----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> MyAccount()
        {
            var user=_context.User.Where(u => u.UserID == tempUserId).Include(u=>u);
            if (user!=null && user.Count() > 0 && user.FirstOrDefault().IsAdmin)
                return RedirectPreserveMethod(nameof(Admin));
            return View();
        }
        //----------------------------------------------------ViewInvoices----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> ViewInvoices()
        {
            return View();
        }
        //----------------------------------------------------ViewGiftCards----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> ViewGiftCards()
        {
            return View();
        }
        //----------------------------------------------------NewGiftCard----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> NewGiftCard()
        {
            return View();
        }
        //----------------------------------------------------NewInvoice----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> NewInvoice()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------

        //
        //
        //
        //
        //                                                    Admin
        //                                                      |
        //                                                      |
        //                                                      V
        //
        //
        //
        //
        //----------------------------------------------------Admin----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> Admin()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AdminViewInvoices()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AdminViewCredits()
        {
            var invoiceProjectContext = _context.Credit.Include(c => c);
            return View(await invoiceProjectContext.ToListAsync());
        }
        [Authorize]
        public async Task<IActionResult> AdminViewGiftCards()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AdminNewInvoice()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AdminNewCredit()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AdminNewGiftCard()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> AdminViewUsers()
        {
            var invoiceProjectContext = _context.User.Include(u=>u);
            return View(await invoiceProjectContext.ToListAsync());
        }
        //----------------------------------------------------DeleteUser----------------------------------------------------
        // GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            return RedirectPreserveMethod(nameof(DeleteConfirmed));
        }

        // POST
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminViewUsers));
        }
        [Authorize]
        public async Task<IActionResult> AdminNewUser()
        {
            return View();
        }
        //----------------------------------------------------Functions----------------------------------------------------
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserID == id);
        }

        //If the user exists by name and password
        private bool Exists(String UserName,String Password)
        {
            return _context.User.Any(u => u.UserName == UserName && u.Password==Password);
        }
    }
}
//----------------------------------------------------Edit----------------------------------------------------
// GET
//public async Task<IActionResult> Edit(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var user = await _context.User.FindAsync(id);
//    if (user == null)
//    {
//        return NotFound();
//    }
//    return View(user);
//}

//// POST
//// To protect from overposting attacks, enable the specific properties you want to bind to, for 
//// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(int id, [Bind("UserID,FirstName,LastName,UserName,Password,IsAdmin,Email,EnteranceDate")] User user)
//{
//    if (id != user.UserID)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
//            _context.Update(user);
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!UserExists(user.UserID))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }
//        return RedirectToAction(nameof(Index));
//    }
//    return View(user);
//}
//----------------------------------------------------Details----------------------------------------------------
//// GET
//public async Task<IActionResult> Details(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }
//    var user = await _context.User
//        .FirstOrDefaultAsync(m => m.UserID == id);
//    if (user == null)
//    {
//        return NotFound();
//    }

//    return View(user);
//}