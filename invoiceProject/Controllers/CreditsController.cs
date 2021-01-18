using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using invoiceProject.Data;
using invoiceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;


namespace invoiceProject.Controllers
{
    public class CreditsController : Controller
    {
        private readonly invoiceProjectContext _context;

        public CreditsController(invoiceProjectContext context)
        {
            _context = context;
        }
        //----------------------------------------------------ViewCredits----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> ViewCredits()
        {
            var invoiceProjectContext = _context.Credit.
                Where(c=> c.UserID == Int32.Parse(HttpContext.Session.GetString("Logged")))
            .Include(u => u.user).Include(c=>c.Category).Select(c=>c);
            return View(await invoiceProjectContext.ToListAsync());
        }
        //----------------------------------------------------NewCredit----------------------------------------------------
        // GET
        [Authorize]
        public IActionResult NewCredit()
        {
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName");
            return View();
        }
        
        //POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> NewCredit([Bind("UserID,CategoryID,CreditID,StoreName,Amount,ExpireDate")] Credit credit)
        {

            if (ModelState.IsValid)
            {
                credit.UserID = Int32.Parse(HttpContext.Session.GetString("Logged"));

                _context.Add(credit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewCredits));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", credit.UserID);
            return View(credit);
        }
        //----------------------------------------------------DeleteCredit----------------------------------------------------
        // GET:
        [Authorize]
        public async Task<IActionResult> DeleteCredit(int? id)
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
        [HttpPost, ActionName("DeleteCredit")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            var credit = await _context.Credit.FindAsync(id);
            _context.Credit.Remove(credit);
            await _context.SaveChangesAsync();
            //if the credit delete frome admin user:
            if (_context.User.Where(u => u.UserID== Int32.Parse(HttpContext.Session.GetString("Logged")))
                .Include(u => u).FirstOrDefault().IsAdmin)
            {
                return RedirectToAction("AdminViewCredits","Users");
            }
            return RedirectToAction(nameof(ViewCredits));
        }
        //----------------------------------------------------EditCredit----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> EditCredit(int? id)
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
        public async Task<IActionResult> EditCredit(int id, [Bind("UserID,CategoryID,CreditID,StoreName,Amount,ExpireDate")] Credit credit)
        {
            if (id != credit.CreditID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditExists(credit.CreditID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewCredits));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", credit.UserID);
            return View(credit);
        }
        //----------------------------------------------------Functions----------------------------------------------------

        private bool CreditExists(int id)
        {
            return _context.Credit.Any(e => e.CreditID == id);
        }
    }
}

