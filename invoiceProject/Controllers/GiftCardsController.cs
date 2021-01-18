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
    public class GiftCardsController : Controller
    {
        private readonly invoiceProjectContext _context;
        
        public GiftCardsController(invoiceProjectContext context)
        {
            _context = context;
        }
        //----------------------------------------------------ViewGiftCards----------------------------------------------------
        // GET: GiftCards
        [Authorize]
        public async Task<IActionResult> ViewGiftCards()
        {
            return View(await _context.UserGiftCard.
                Where(ugc=>ugc.UserID== Int32.Parse(HttpContext.Session.GetString("Logged")))
                .Include(g=>g.giftCard).ToListAsync());
        }
        //----------------------------------------------------NewGiftCard----------------------------------------------------
        // GET:
        [Authorize]
        public IActionResult NewGiftCard()
        {
            return View();
        }

        // POST
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewGiftCard(string GiftCardName, double Price)
        {
            
                var user = _context.User.
                    Where(u => u.UserID == Int32.Parse(HttpContext.Session.GetString("Logged"))).Select(u => u).FirstOrDefault();

                var giftCard = _context.GiftCard.Where(g => g.GiftCardName== GiftCardName && g.Price==Price)
                    .Select(u => u).FirstOrDefault();

                if (!GiftCardExists(user.UserID , GiftCardName , Price))
                {
                    var userGiftCard = new UserGiftCard() {
                        Count = 1,
                        UserID=user.UserID,
                        GiftCardID=giftCard.GiftCardID,
                    };
                     _context.UserGiftCard.Add(userGiftCard);
                }
                else
                {
                    _context.UserGiftCard.Where(u => u.UserID == user.UserID).FirstOrDefault().Count++;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewGiftCards));
        }
        //----------------------------------------------------DeleteGiftCard----------------------------------------------------
        // GET:
        [Authorize]
        public async Task<IActionResult> DeleteGiftCard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userGiftCard = await _context.UserGiftCard
                .Where(g => g.GiftCardID == id && g.UserID== Int32.Parse(HttpContext.Session.GetString("Logged")))
                .FirstOrDefaultAsync();
            if (userGiftCard == null)
            {
                return NotFound();
            }

            return View();
        }

        // POST:
        [Authorize]
        [HttpPost, ActionName("DeleteGiftCard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userGiftCard = await _context.UserGiftCard.
                Where(g => g.GiftCardID == id && g.UserID == Int32.Parse(HttpContext.Session.GetString("Logged")))
                .FirstOrDefaultAsync();

            if (userGiftCard.Count == 1)
            {
                _context.UserGiftCard.Remove(userGiftCard);
            }
            else
            {
                userGiftCard.Count--;
            }
            await _context.SaveChangesAsync();

            //if the credit delete frome admin user:
            if (_context.User.Where(u => u.UserID == Int32.Parse(HttpContext.Session.GetString("Logged"))).
                Select(u => u).FirstOrDefault().IsAdmin)
            {
                return RedirectToAction("AdminViewGiftCards", "Users");
            }
            return RedirectToAction(nameof(ViewGiftCards));
        }
        //----------------------------------------------------EditGiftCard----------------------------------------------------
        [Authorize]
        // GET: GiftCards/Edit/5
        public async Task<IActionResult> EditGiftCard(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var userGiftCard = await _context.UserGiftCard.
                Where(ugc=>ugc.UserID== Int32.Parse(HttpContext.Session.GetString("Logged")) && ugc.GiftCardID==id)
                .Select(ugc=>ugc).FirstOrDefaultAsync();

            if (userGiftCard == null)
            {
                return NotFound();
            }
            return View(userGiftCard);
        }

        //POST:
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGiftCard(int id, string GiftCardName, double Price)
        {
            //The exist giftCard:
            var userGiftCard = await _context.UserGiftCard.
                Where(g => g.GiftCardID == id && g.UserID== Int32.Parse(HttpContext.Session.GetString("Logged")))
                .Select(g=>g).FirstOrDefaultAsync();

            //The wanted giftCard:
            var wantedGiftCard = _context.GiftCard.Where(g => g.GiftCardName == GiftCardName && g.Price == Price).
            Select(g => g).FirstOrDefault();

            //If the user make "edit" with the same deatails:
            if (userGiftCard.GiftCardID == wantedGiftCard.GiftCardID)
                return RedirectToAction(nameof(ViewGiftCards));

            //If the user make "edit" with others deatails:
            else
            {
                //Delete the old wrong giftCard:
                if (userGiftCard.Count > 1)
                {
                    userGiftCard.Count--;
                    _context.Update(userGiftCard);
                }
                else
                {
                    _context.UserGiftCard.Remove(userGiftCard);
                }

                //Creating the new giftCard and we have 2 options:
                // 1. If the user has this giftCard already we will update the count:
                if (GiftCardExists(Int32.Parse(HttpContext.Session.GetString("Logged")), GiftCardName, Price))
                {
                    var ugc= _context.UserGiftCard.Where(ugc => ugc.UserID == Int32.Parse(HttpContext.Session.GetString("Logged"))
                    && ugc.GiftCardID== wantedGiftCard.GiftCardID).Select(ugc => ugc).FirstOrDefault();

                    ugc.Count++;
                    _context.Update(ugc);

                }
                // 2. If the user doesn't have this giftCard we will creat a new one:
                else
                {
                    var newUserGiftCard = new UserGiftCard()
                    {
                        Count = 1,
                        UserID = Int32.Parse(HttpContext.Session.GetString("Logged")),
                        GiftCardID = wantedGiftCard.GiftCardID,
                    };
                    _context.UserGiftCard.Add(newUserGiftCard);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewGiftCards));
        }

        private bool GiftCardExists(int UserID,string GiftCardName,double Price)
        {
            var giftCard = _context.GiftCard.Where(g => g.GiftCardName == GiftCardName && g.Price == Price).
                Select(g => g).FirstOrDefault();
            return _context.UserGiftCard.Any(ugc => ugc.GiftCardID == giftCard.GiftCardID && ugc.UserID==UserID);
            
        }
    }
}






