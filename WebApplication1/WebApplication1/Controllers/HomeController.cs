using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string Valor)
        {
            if(Valor != null)
            {
                ViewBag.id = Valor;
                return View("Call");
            }
            return View();
        }

        public IActionResult Index()
        {
            ViewBag.id = "Default";
            return View();
        }

       [HttpGet]
        public IActionResult Call(string Valor)
        {
            if(Valor != null) 
            {
                ViewBag.id = Valor;
            }
            else { return View("Index"); }

            
            return View();
        }


        [HttpGet]
        public IActionResult EndCall(string Sec, string Min, bool Llamada)
        {
            ViewBag.Llamada = Llamada;
            ViewBag.Min = Min;
            ViewBag.Sec = Sec;
            return View();
        }

        public IActionResult EndCall()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
