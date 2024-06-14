using DongHoDemo2.Models;
using DongHoDemo2.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DongHoDemoContext _context;
        public HomeController(ILogger<HomeController> logger, DongHoDemoContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //HomeVM model = new HomeVM();
            var listProducts = _context.Products
                .AsNoTracking()
                .Where(x => x.UnitslnStock > 0)
                .OrderByDescending(x => x.DateCreated)
                .Take(8)
                .ToList();

            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            ViewBag.AllProducts = listProducts;
            return View();
        }
        public IActionResult Contact()
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            return View();
        }
        public IActionResult ShopCart()
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            return View();
        }
        public IActionResult Privacy()
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
