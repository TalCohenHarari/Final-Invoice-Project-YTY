﻿using System;
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

            //ViewBag.price = (from userGiftCard in _context.UserGiftCard
            //                 join giftcard in _context.GiftCard
            //                 on userGiftCard.GiftCardID equals giftcard.GiftCardID
            //                 where userGiftCard.UserID == UsersController.tempUserId
            //                 select userGiftCard).FirstOrDefault().price;

            var GiftCardList = from userGiftCard in _context.UserGiftCard
                               join giftCard in _context.GiftCard
                               on userGiftCard.GiftCardID equals giftCard.GiftCardID 
                               where userGiftCard.UserID==UsersController.tempUserId
                               select giftCard;

            return View(await GiftCardList.ToListAsync());
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
        public async Task<IActionResult> NewGiftCard([Bind("GiftCardName,ExpireDate")] GiftCard giftCard,double price)
        {
            if (ModelState.IsValid)
            {
                _context.GiftCard.Add(giftCard);
                await _context.SaveChangesAsync();

                var user = _context.User.Where(u => u.UserID == UsersController.tempUserId).Select(u => u).FirstOrDefault();
                
                var userGiftCard =new UserGiftCard() { 
                    GiftCardID=giftCard.GiftCardID,
                    UserID= user.UserID,
                    price= price
                };
                _context.UserGiftCard.Add(userGiftCard);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ViewGiftCards));
            }
            return View();
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

            var giftCard = await _context.GiftCard
                .FirstOrDefaultAsync(m => m.GiftCardID == id);
            if (giftCard == null)
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
            var giftCard = await _context.GiftCard.FindAsync(id);
            _context.GiftCard.Remove(giftCard);
            await _context.SaveChangesAsync();
            //if the credit delete frome admin user:
            if (_context.User.Where(u => u.UserID == UsersController.tempUserId).Include(u => u).FirstOrDefault().IsAdmin)
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

            var giftCard = await _context.GiftCard.FindAsync(id);
            if (giftCard == null)
            {
                return NotFound();
            }
            return View(giftCard);
        }

        //POST:
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGiftCard(int id, [Bind("GiftCardID,GiftCardName,ExpireDate")] GiftCard giftCard,double price)
        {

            if (id != giftCard.GiftCardID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    (from userGiftCard in _context.UserGiftCard
                     where  giftCard.GiftCardID == userGiftCard.GiftCardID
                     select userGiftCard).First().price = price;

                    _context.Update(giftCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiftCardExists(giftCard.GiftCardID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ViewGiftCards));
            }
            return View(giftCard);
        }

        private bool GiftCardExists(int id)
        {
            return _context.GiftCard.Any(e => e.GiftCardID == id);
        }
    }
}







//// GET: GiftCards/Details/5
//public async Task<IActionResult> Details(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }

//    var giftCard = await _context.GiftCard
//        .FirstOrDefaultAsync(m => m.GiftCardID == id);
//    if (giftCard == null)
//    {
//        return NotFound();
//    }

//    return View(giftCard);
//}