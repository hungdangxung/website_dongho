using DongHoDemo2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.Controllers
{
    public class SearchController : Controller
    {
        private readonly DongHoDemoContext _context;
        public SearchController(DongHoDemoContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        //Tim kiem san pham
        [HttpGet]
        public IActionResult SearchProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearchPartialView", null);
            }
            ls = _context.Products
                .Include(x => x.Cat)
                .AsNoTracking()
                .Where(x => x.ProductName.Contains(keyword))
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListProductsSearchPartialView", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartialView", ls);
            }
        }

        //Loc san pham
        [HttpGet]
        public IActionResult FilterProduct(int idRange)
        {
            List<Product> ls = new List<Product>();
            switch (idRange)
            {
                case 1:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .Where(x => (x.Discount >= 0 && x.Discount <= 100000) || ( x.Price >= 0 && x.Price <= 100000))
                    .OrderByDescending(x => x.ProductName)
                    .Take(9)
                    .ToList();
                    break;
                case 2:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .Where(x => (x.Discount >= 100000 && x.Discount <= 500000) || (x.Price >= 100000 && x.Price <= 500000))
                    .OrderByDescending(x => x.ProductName)
                    .Take(9)
                    .ToList();
                    break;
                case 3:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .Where(x => (x.Discount >= 500000 && x.Discount <= 1000000) || (x.Price >= 500000 && x.Price <= 1000000))
                    .OrderByDescending(x => x.ProductName)
                    .Take(9)
                    .ToList();
                    break;
                case 4:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .Where(x => (x.Discount >= 1000000 && x.Discount <= 5000000) || (x.Price >= 1000000 && x.Price <= 5000000))
                    .OrderByDescending(x => x.ProductName)
                    .Take(9)
                    .ToList();
                    break;
                case 5:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .Where(x => (x.Discount > 5000000) || (x.Price > 5000000))
                    .OrderByDescending(x => x.ProductName)
                    .Take(9)
                    .ToList();
                    break;
                case 6:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .Take(9)
                    .ToList();
                    break;
            }
            if (ls == null)
            {
                return PartialView("ListProductsFilterPartialView", null);
            }
            else
            {
                return PartialView("ListProductsFilterPartialView", ls);
            }
        }

        //Sap xep san pham
        [HttpGet]
        public IActionResult OrderByProduct(int idOrder)
        {
            List<Product> ls = new List<Product>();
            switch (idOrder)
            {
                //Sap xep theo ten tu A-Z
                case 1:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .OrderBy(x => x.ProductName)
                    .Take(9)
                    .ToList();
                    break;

                //Sap xep theo ten tu Z-A
                case 2:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .OrderByDescending(x => x.ProductName)
                    .Take(9)
                    .ToList();
                    break;

                //Sap xep theo gia tang dan
                case 3:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .OrderBy(x => x.Price)
                    .Take(9)
                    .ToList();
                    break;

                //Sap xep theo gia giam dan
                case 4:
                    ls = _context.Products
                    .Include(x => x.Cat)
                    .AsNoTracking()
                    .OrderByDescending(x => x.Price)
                    .Take(9)
                    .ToList();
                    break;
            }
            if (ls == null)
            {
                return PartialView("ListProductsOrderPartialView", null);
            }
            else
            {
                return PartialView("ListProductsOrderPartialView", ls);
            }
        }
    }
}
