using DongHoDemo2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.Controllers
{
    public class ProductController : Controller
    {
        private readonly DongHoDemoContext _context;
        public ProductController(DongHoDemoContext context)
        {
            _context = context;
        }
        [Route("Shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 9;
                var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
                var products = _context.Products
                    .AsNoTracking()
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(products, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.AllCategories = listCats;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("/{CatName}", Name = "ListProducts")]
        public IActionResult ListCat(string catName, int page = 1)
        {
            try
            {
                var pageSize = 9;
                var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.CatName == catName);
                var products = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == danhmuc.CatId)
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(products, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc;
                ViewBag.AllCategories = listCats;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("/{ProductName}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.ProductId != id)
                    .OrderByDescending(x => x.DateCreated)
                    .Take(8)
                    .ToList();
                var listCats = _context.Categories.Include(c => c.Products)
                    .AsNoTracking()
                    .Take(6)
                    .ToList();
                var listColors = _context.Colors
                    .AsNoTracking()
                    .ToList();

                ViewBag.AllCategories = listCats;
                ViewBag.SanPham = lsProducts;
                ViewBag.Color = listColors;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
