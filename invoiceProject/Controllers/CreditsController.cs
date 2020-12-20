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

namespace invoiceProject.Controllers
{
    public class CreditsController : Controller
    {
        private readonly invoiceProjectContext _context;

        public CreditsController(invoiceProjectContext context)
        {
            _context = context;
        }

        // GET: Credits
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var invoiceProjectContext = _context.Credit.Include(c => c.user);
            return View(await invoiceProjectContext.ToListAsync());
        }

        // GET: Credits/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(credit);
        }

        // GET: Credits/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName");
            return View();
        }

        // POST: Credits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("UserID,CreditID,StoreName,Amount,ExpireDate")] Credit credit)
        {
            if (ModelState.IsValid)
            {
                credit.UserID = Int32.Parse(TempData["UserID"].ToString());
                _context.Add(credit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", credit.UserID);
            return View(credit);
        }

        // GET: Credits/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Credits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,CreditID,StoreName,Amount,ExpireDate")] Credit credit)
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", credit.UserID);
            return View(credit);
        }

        // GET: Credits/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
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
        
            return RedirectPreserveMethod("DeleteConfirm");
        }

        // POST: Credits/Delete/5
        [HttpPost, ActionName("DeleteConfirm")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            var credit = await _context.Credit.FindAsync(id);
            _context.Credit.Remove(credit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditExists(int id)
        {
            return _context.Credit.Any(e => e.CreditID == id);
        }
    }
}
