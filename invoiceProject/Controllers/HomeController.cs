using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace invoiceProject.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index() 
        {
            //if (HttpContext.Session.GetString("Logged") != null)
            //{
            //    return RedirectToAction("MyAccount", "Users");
            //}
            return View();
        }
        //GET
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }

    }
}
