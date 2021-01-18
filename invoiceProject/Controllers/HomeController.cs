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
