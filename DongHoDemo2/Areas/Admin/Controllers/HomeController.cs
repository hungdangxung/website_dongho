using DongHoDemo2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly DongHoDemoContext _context;

        public HomeController(DongHoDemoContext context)
        {
            _context = context;
        }
        [Route("Admin")]
        [Authorize]
        public IActionResult Index()
        {
            var listOrdereds = _context.Orders
                .AsNoTracking()
                .Where(x => (x.TransactStatusId == 1005 || x.TransactStatusId == 4))
                .ToList();
            var listDayOrdereds = _context.Orders
                .AsNoTracking()
                .Where(x => (x.TransactStatusId == 1005 || x.TransactStatusId == 4) && x.ShipDate.Value.Date == DateTime.Now.Date)
                .ToList();
            var listMonthOrdereds = _context.Orders
                .AsNoTracking()
                .Where(x => (x.TransactStatusId == 1005 || x.TransactStatusId == 4) && x.ShipDate.Value.Month == DateTime.Now.Month)
                .ToList();
            var list3MonthOrdereds = _context.Orders
                .AsNoTracking()
                .Where(x => (x.TransactStatusId == 1005 || x.TransactStatusId == 4) && ((x.ShipDate.Value.Month - 1)/3)== DateTime.Now.Month)
                .ToList();
            var listYearOrdereds = _context.Orders
                .AsNoTracking()
                .Where(x => (x.TransactStatusId == 1005 || x.TransactStatusId == 4) && x.ShipDate.Value.Year == DateTime.Now.Year)
                .ToList();
            var listOrders = _context.Orders
                .AsNoTracking()
                .ToList();
            var listProducts = _context.Products
                .AsNoTracking()
                .ToList();
            var listCustomers = _context.Customers
                .AsNoTracking()
                .ToList();
            ViewBag.listOrdereds = listOrdereds;
            ViewBag.listDayOrdereds = listDayOrdereds;
            ViewBag.listMonthOrdereds = listMonthOrdereds;
            ViewBag.list3MonthOrdereds = list3MonthOrdereds;
            ViewBag.listYearOrdereds = listYearOrdereds;
            ViewBag.listOrders = listOrders;
            ViewBag.listProducts = listProducts;
            ViewBag.listCustomers = listCustomers;
            return View();
        }
    }
}
