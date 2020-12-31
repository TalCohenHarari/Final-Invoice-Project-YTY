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
    public class InvoicesController : Controller
    {
        private readonly invoiceProjectContext _context;

        public InvoicesController(invoiceProjectContext context)
        {
            _context = context;
        }
        //----------------------------------------------------ViewInvoices----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> ViewInvoices()
        {
            var invoiceProjectContext = _context.Invoice.Where(c => c.UserID == Int32.Parse(HttpContext.Session.GetString("Logged")))
            .Include(c => c.user);
            return View(await invoiceProjectContext.ToListAsync());
        }
        //----------------------------------------------------Newinvoice----------------------------------------------------
        // GET
        [Authorize]
        public IActionResult NewInvoice()
        {
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName");
            return View();
        }

        // POST
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> NewInvoice([Bind("InvoiceID,StoreName,PurchaseDate,Amount,CategoryID,ExpireDate")] Invoice invoice)
        {
            if (invoice!=null)
            {
                invoice.UserID = Int32.Parse(HttpContext.Session.GetString("Logged"));
                _context.Add(invoice);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewInvoices));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", invoice.UserID);
            return View(invoice);
        }
        //----------------------------------------------------DeleteInvoice----------------------------------------------------
        // GET:
        [Authorize]
        public async Task<IActionResult> DeleteInvoice(int? id)
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
        [HttpPost, ActionName("DeleteInvoice")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {
            var invoice = await _context.Invoice.FindAsync(id);
            _context.Invoice.Remove(invoice);
            await _context.SaveChangesAsync();
            //if the credit delete frome admin user:
            if (_context.User.Where(u => u.UserID == Int32.Parse(HttpContext.Session.GetString("Logged"))).
                Include(u => u).FirstOrDefault().IsAdmin)
            {
                return RedirectToAction("AdminViewIncoices", "Users");
            }
            return RedirectToAction(nameof(ViewInvoices));
        }
        //----------------------------------------------------EditInvoice----------------------------------------------------
        // GET
        [Authorize]
        public async Task<IActionResult> EditInvoice(int? id)
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
        public async Task<IActionResult> EditInvoice(int id, [Bind("UserID,InvoiceID,StoreName,PurchaseDate,Amount,CategoryID,ExpireDate")] Invoice invoice)
        {
            if (id != invoice.InvoiceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewInvoices));
            }
            ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", invoice.UserID);
            return View(invoice);
        }
        private bool InvoiceExists(int id)
        {
            return _context.Invoice.Any(e => e.InvoiceID == id);
        }

    }
}




//// GET: Invoices
//public async Task<IActionResult> Index()
//{
//    var invoiceProjectContext = _context.Invoice.Include(i => i.user);
//    return View(await invoiceProjectContext.ToListAsync());
//}

//// GET: Invoices/Details/5
//public async Task<IActionResult> Details(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var invoice = await _context.Invoice
//        .Include(i => i.user)
//        .FirstOrDefaultAsync(m => m.InvoiceID == id);
//    if (invoice == null)
//    {
//        return NotFound();
//    }

//    return View(invoice);
//}

//// GET: Invoices/Create
//public IActionResult Create()
//{
//    ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName");
//    return View();
//}

//// POST: Invoices/Create
//// To protect from overposting attacks, enable the specific properties you want to bind to, for 
//// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Create([Bind("UserID,InvoiceID,StoreName,PurchaseDate,Amount,CategoryID,ExpireDate")] Invoice invoice)
//{
//    if (ModelState.IsValid)
//    {
//        _context.Add(invoice);
//        await _context.SaveChangesAsync();
//        return RedirectToAction(nameof(Index));
//    }
//    ViewData["UserID"] = new SelectList(_context.User, "UserID", "FirstName", invoice.UserID);
//    return View(invoice);
//}



//// GET: Invoices/Delete/5
//public async Task<IActionResult> Delete(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var invoice = await _context.Invoice
//        .Include(i => i.user)
//        .FirstOrDefaultAsync(m => m.InvoiceID == id);
//    if (invoice == null)
//    {
//        return NotFound();
//    }

//    return View(invoice);
//}

//// POST: Invoices/Delete/5
//[HttpPost, ActionName("Delete")]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> DeleteConfirmed(int id)
//{
//    var invoice = await _context.Invoice.FindAsync(id);
//    _context.Invoice.Remove(invoice);
//    await _context.SaveChangesAsync();
//    return RedirectToAction(nameof(Index));
//}

//private bool InvoiceExists(int id)
//{
//    return _context.Invoice.Any(e => e.InvoiceID == id);
//}