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
        //public static int tempUserId;
        public UsersController(invoiceProjectContext context)
        {
            _context = context;
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

                HttpContext.Session.SetString("Logged", user.UserID.ToString());
                SignIn(user);
                return RedirectToAction(nameof(MyAccount));
            }
            else
            {
                ViewData["RegisterError"] = "משתמש זה כבר קיים במערכת";
            }
            return View();
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
            var user = _context.User.Where(u => u.UserName == UserName && u.Password == Password).Select(u=>u);
            
            if (user != null && user.Count() > 0)
            {
                HttpContext.Session.SetString("Logged",user.FirstOrDefault().UserID.ToString());

                if (user.FirstOrDefault().IsAdmin)
                {
                    SignIn(user.First());
                    return RedirectToAction(nameof(Admin));
                }
                SignIn(user.First());
                return RedirectToAction(nameof(MyAccount));
            }
            else
            {
                ViewData["LoginError"] = "השם משתמש או הסיסמה אינם נכונים";
            }
            return View();
         }
        //----------------------------------------------------SignIn----------------------------------------------------
        private async void SignIn(User user)
        {
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
        //----------------------------------------------------Logout----------------------------------------------------
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Logged");

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        //----------------------------------------------------MyAccount----------------------------------------------------
        //[Authorize]
        public async Task<IActionResult> MyAccount()
        {
            //If someone click on "myAccount" link and he isn't logged in it's redirect him to login page:
            if (HttpContext.Session.GetString("Logged") == null)
                return RedirectToAction(nameof(Login));

            //Statistic calculate of splineArea:
            ViewBag.January = calculateMoneyPerMonth(1);
            ViewBag.April = calculateMoneyPerMonth(4);
            ViewBag.August = calculateMoneyPerMonth(8);
            ViewBag.December = calculateMoneyPerMonth(12);

            //Statistic calculate of doughnut:
            ViewBag.Food = calculateMoneyPerCategory("מזון");
            ViewBag.Clothes = calculateMoneyPerCategory("ביגוד והנעלה");
            ViewBag.ElectricPower = calculateMoneyPerCategory("חשמל");
            ViewBag.Fuel = calculateMoneyPerCategory("דלק");
            ViewBag.insurance = calculateMoneyPerCategory("ביטוח");
           
         
            //Now we sum all the invoices money that current user have, and show him it:
            var list = _context.Invoice.Where(u=>u.UserID == Int32.Parse(HttpContext.Session.GetString("Logged")))
                .Select(u => u.Amount).ToList();
            long sum = 0;
            foreach (var item in list)
                sum += long.Parse(item.ToString());
            ViewBag.MoneySumOfAllInvoices = sum;

            //Now we count all the invoices that current user have, and show him it:
            ViewBag.AmountOfAllInvoices = _context.Invoice.
                Where(u => u.UserID == Int32.Parse(HttpContext.Session.GetString("Logged"))).Count();

            //If it's the Admin:
            var user =_context.User.
                Where(u => u.UserID == Int32.Parse(HttpContext.Session.GetString("Logged"))).Include(u=>u);
            if (user!=null && user.Count() > 0 && user.FirstOrDefault().IsAdmin)
                return RedirectPreserveMethod(nameof(Admin));

            return View();
        }
        //Calculating the money per month, "splineArea" type:
        public double calculateMoneyPerMonth(int month)
        {
            var list = (from i in _context.Invoice
                        where i.PurchaseDate.Month == month && i.UserID == Int32.Parse(HttpContext.Session.GetString("Logged"))
                        select i).ToList();
            double money = 0;
            foreach (var item in list)
                money += item.Amount;

            return money;
        }
        //Calculating all the money according to the specific category, "doughnut" type:
        public double calculateMoneyPerCategory(string categoryName)
        {
            var list = (from i in _context.Invoice
                        where i.Category.CategoryName == categoryName 
                            && i.UserID== Int32.Parse(HttpContext.Session.GetString("Logged"))
                        select i).ToList();
            double money = 0;
            foreach (var item in list)
                money += item.Amount;

            return money;
        }
        //----------------------------------------------------EditMyAccount----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> EditMyAccount(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMyAccount(int UserID,string FirstName, string LastName,string Password,string Email)
        {
            var user = _context.User.Where(u => u.UserID == UserID).Select(u => u).FirstOrDefault();
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Password = Password;
            user.Email = Email;
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyAccount));
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

            //Count of the new users in the site per month:
            ViewBag.newUsersJanuary = usersPerMonth(1);
            ViewBag.newUsersFebruary = usersPerMonth(2);
            ViewBag.newUsersMarch = usersPerMonth(3);
            ViewBag.newUsersApril = usersPerMonth(4);
            ViewBag.newUsersMay = usersPerMonth(5);
            ViewBag.newUsersJune = usersPerMonth(6);
            ViewBag.newUsersJulay = usersPerMonth(7);
            ViewBag.newUsersAugust = usersPerMonth(8);
            ViewBag.newUsersSeptember  = usersPerMonth(9);
            ViewBag.newUsersOctober = usersPerMonth(10);
            ViewBag.newUsersNovember = usersPerMonth(11);
            ViewBag.newUsersDecember= usersPerMonth(12);

            return View();
        }
        public int usersPerMonth(int month)
        {
            return _context.User.Where(u => u.EnteranceDate.Month == month).Select(u => u).Count();
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
            return View(await _context.Invoice.Include(u => u.user).Include(u => u.Category).Select(i=>i).ToListAsync());
        }
        //----------------------------------------------------AdminViewCredits----------------------------------------------------
        [Authorize]
        public async Task<IActionResult> AdminViewCredits()
        {
            return View(await _context.Credit.Include(u=>u.user).Include(u => u.Category).Select(c => c).ToListAsync());
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
            else
            {
                ViewData["AdminNewUserError"] = "משתמש זה כבר קיים במערכת";
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
        public async Task<IActionResult> AdminNewInvoice([Bind("UserID,InvoiceID,StoreName,PurchaseDate,Amount,CategoryID,ExpireDate")] Invoice invoice)
        {
            if (invoice!=null)
            {
                _context.Add(invoice);
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
        public async Task<IActionResult> AdminNewCredit([Bind("UserID,CreditID,CategoryID,StoreName,Amount,ExpireDate")] Credit credit)
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
        //----------------------------------------------------AdminEditUser----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> AdminEditUser(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEditUser([Bind("UserID,FirstName,LastName,UserName,Password,IsAdmin,Email,EnteranceDate")] User user)
        {
            //Search if there is the same userName like the "new" one, and jump on the current user:
            var userNameIsValid = _context.User.Where(u => u.UserName == user.UserName && u.UserID!=user.UserID).Select(u => u);

            if (ModelState.IsValid && !(userNameIsValid.Count()>0))
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminViewUsers));
            }
            else
            {
                ViewData["AdminEditUserError"] = "שם משתמש זה כבר קיים במערכת";
            }
            return View(user);
        }
        //----------------------------------------------------AdminEditInvoice----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> AdminEditInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoice.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", invoice.UserID);
            return View(invoice);
        }

        // POST:
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEditInvoice(int id, [Bind("UserID,InvoiceID,StoreName,PurchaseDate,Amount,CategoryID,ExpireDate")] Invoice invoice)
        {
            if (id != invoice.InvoiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
               
                return RedirectToAction(nameof(AdminViewInvoices));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", invoice.UserID);
            return View(invoice);
        }
        //----------------------------------------------------AdminEditCredit----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> AdminEditCredit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credit = await _context.Credit.FindAsync(id);
            if (credit == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", credit.UserID);
            return View(credit);
        }

        // POST:
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEditCredit(int id, [Bind("UserID,CategoryID,CreditID,StoreName,Amount,ExpireDate")] Credit credit)
        {
            if (id != credit.CreditID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(credit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AdminViewCredits));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", credit.UserID);
            return View(credit);
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