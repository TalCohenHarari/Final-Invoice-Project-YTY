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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserID,FirstName,LastName,UserName,Password,IsAdmin,Email,EnteranceDate")] User user)
        {
            if (ModelState.IsValid && !Exists(user.UserName))
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
            var list = _context.Invoice.Where(u=>u.UserID==tempUserId).Select(u => u.Amount).ToList();
            int sum = 0;
            foreach (var item in list)
                sum += Int32.Parse(item.ToString());
            ViewBag.MoneySumOfAllInvoices = sum;

            ViewBag.AmountOfAllInvoices = _context.Invoice.Where(u => u.UserID == tempUserId).Count();

            var user=_context.User.Where(u => u.UserID == tempUserId).Include(u=>u);
            if (user!=null && user.Count() > 0 && user.FirstOrDefault().IsAdmin)
                return RedirectPreserveMethod(nameof(Admin));
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------
        //
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
        //----------------------------------------------------AdminPage----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> Admin()
        {
            ViewBag.AdminAmountOfAllInvoices = _context.Invoice.Count();
            ViewBag.AdminAmountOfAllUsers = _context.User.Count();

            return View();
        }
        //----------------------------------------------------AdminViewUsers----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> AdminViewUsers()
        {
            return View(await _context.User.ToListAsync());
        }
        //----------------------------------------------------AdminViewInvoices----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> AdminViewInvoices()
        {
            return View(await _context.Invoice.Include(u => u.user).ToListAsync());
        }
        //----------------------------------------------------AdminViewCredits----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> AdminViewCredits()
        {
            return View(await _context.Credit.Include(u=>u.user).ToListAsync());
        }
        //----------------------------------------------------AdminViewGiftCards----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> AdminViewGiftCards()
        {
            return View(await _context.UserGiftCard.Include(g=>g.giftCard).Include(u=>u.user).ToListAsync());
        }
        //----------------------------------------------------AdminNewUser----------------------------------------------------
        //GET
        [Authorize]
        public async Task<IActionResult> AdminNewUser()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminNewUser([Bind("UserID,FirstName,LastName,UserName,Password,IsAdmin,Email,EnteranceDate")] User user)
        {
            if (ModelState.IsValid && !Exists(user.UserName))
            {
                //user.EnteranceDate = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminViewUsers));
            }
            return View();
        }
        //----------------------------------------------------AdminNewInvoice----------------------------------------------------
        //GET
        [Authorize]
        public async Task<IActionResult> AdminNewInvoice()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AdminNewInvoice([Bind("UserID,InvoiceID,StoreName,PurchaseDate,Amount,CategoryID,ExpireDate")] Invoice invoice,string radio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);

                //If radio=="1" creat a new Credit too:
                if (radio == "1")
                {
                    Credit newCredit = new Credit();
                    newCredit.StoreName = invoice.StoreName;
                    newCredit.CategoryID = invoice.CategoryID;
                    newCredit.ExpireDate = invoice.ExpireDate;
                    newCredit.Amount = invoice.Amount;
                    newCredit.UserID = invoice.UserID;

                    _context.Credit.Add(newCredit);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(AdminViewInvoices));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", invoice.UserID);
            return View(invoice);
        }
        //----------------------------------------------------AdminNewCredit----------------------------------------------------
        //GET:
        [Authorize]
        public async Task<IActionResult> AdminNewCredit()
        {
            return View();
        }
        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AdminNewCredit([Bind("UserID,CreditID,StoreName,Amount,ExpireDate")] Credit credit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(credit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminViewCredits));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", credit.UserID);
            return View(credit);
        }
        //----------------------------------------------------AdminNewGiftCard----------------------------------------------------
        //GET:
        [Authorize]
        public async Task<IActionResult> AdminNewGiftCard()
        {
            return View();
        }
        //POST:
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminNewGiftCard( int UserID, string GiftCardName, double Price)
        {
            var user = _context.User.Where(u => u.UserID == UserID).Select(u => u).FirstOrDefault();

            var giftCard = _context.GiftCard.Where(g => g.GiftCardName == GiftCardName && g.Price == Price)
                .Select(u => u).FirstOrDefault();

            if (!GiftCardExists(user.UserID, GiftCardName, Price))
            {
                var userGiftCard = new UserGiftCard()
                {
                    Count = 1,
                    UserID = user.UserID,
                    GiftCardID = giftCard.GiftCardID,
                };

                _context.UserGiftCard.Add(userGiftCard);
            }
            else
            {
                _context.UserGiftCard.Where(u => u.UserID == user.UserID).FirstOrDefault().Count++;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminViewGiftCards));
        }
        //----------------------------------------------------AdminDeleteUser----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> AdminDeleteUser(int? id)
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

            return View();
        }

        // POST
        [Authorize]
        [HttpPost, ActionName("AdminDeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminViewUsers));
        }
        //----------------------------------------------------AdminDeleteInvoice----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> AdminDeleteInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice
                .Include(c => c.user)
                .FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View();
        }
        // POST:
        [HttpPost, ActionName("AdminDeleteInvoice")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            var invoice = await _context.Invoice.FindAsync(id);
            _context.Invoice.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminViewInvoices));
        }
        //----------------------------------------------------AdminDeleteCredit----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> AdminDeleteCredit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credit = await _context.Credit
                .Include(c => c.user)
                .FirstOrDefaultAsync(m => m.CreditID == id);
            if (credit == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST:
        [HttpPost, ActionName("AdminDeleteCredit")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirme(int? id)
        {
            var credit = await _context.Credit.FindAsync(id);
            _context.Credit.Remove(credit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdminViewCredits));
        }
        //----------------------------------------------------AdminDeleteGiftCard----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> AdminDeleteGiftCard(int? userID,int? giftCardID)
        {
            if (userID == null && giftCardID==null)
            {
                return NotFound();
            }

            var userGiftCard = await _context.UserGiftCard
                .FirstOrDefaultAsync(g => g.GiftCardID == giftCardID && g.UserID == userID);
            if (userGiftCard == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST:
        [Authorize]
        [HttpPost, ActionName("AdminDeleteGiftCard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmd(int userID, int giftCardID)
        {
            var userGiftCard = await _context.UserGiftCard.FindAsync(userID, giftCardID);
            _context.UserGiftCard.Remove(userGiftCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AdminViewGiftCards));
        }
        //----------------------------------------------------Functions----------------------------------------------------
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserID == id);
        }

        //If the user exists by UserName:
        private bool Exists(String UserName)
        {
            return _context.User.Any(u => u.UserName == UserName);
        }
        //If the GiftCard exists:
        private bool GiftCardExists(int UserID, string GiftCardName, double Price)
        {
            var giftCard = _context.GiftCard.Where(g => g.GiftCardName == GiftCardName && g.Price == Price).
                Select(g => g).FirstOrDefault();
            return _context.UserGiftCard.Any(ugc => ugc.GiftCardID == giftCard.GiftCardID && ugc.UserID==UserID);

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