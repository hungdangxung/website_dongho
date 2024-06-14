using AspNetCoreHero.ToastNotification.Abstractions;
using DongHoDemo2.Extensions;
using DongHoDemo2.Models;
using DongHoDemo2.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly DongHoDemoContext _context;
        public INotyfService _notiyfService { get; }
        public ShoppingCartController(DongHoDemoContext context, INotyfService notyfService)
        {
            _context = context;
            _notiyfService = notyfService;
        }
        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }
        /*
            1. Thêm mới sản phẩm vào giỏ hàng
            2. Cập nhật lại số lượng giỏ hàng
            3. Xóa sản phẩm khỏi giỏ hàng
            4. Xóa luôn giỏ hàng
        */
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? quantity, int? colorID)
        {
            List<CartItem> gioHang = GioHang;
            try
            {
                if(colorID == null)
                {
                    colorID = 1;
                }
                //Them san pham vao gio hang
                CartItem item = GioHang.SingleOrDefault(p => p.product.ProductId == productID && p.color.ColorId == colorID);
                if (item != null) // da co => them so luong
                {
                    if (quantity.HasValue)
                    {
                        item.quantity = quantity.Value;
                    }
                    else
                    {
                        item.quantity++;
                    }
                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    Color colorCart = _context.Colors.SingleOrDefault(p => p.ColorId == colorID);
                    item = new CartItem
                    {
                        quantity = quantity.HasValue ? quantity.Value : 1,
                        color = colorCart,
                        product = hh
                    };
                    gioHang.Add(item);
                }

                //Luu session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notiyfService.Success("Thêm sản phẩm thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && quantity.HasValue) // da co => them so luong
                    {
                        item.quantity = quantity.Value;
                    }
                    //Luu session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                    _notiyfService.Success("Thêm sản phẩm thành công");
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult RemoveCart(int productID)
        {
            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                // luu lai session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            //List<int> listProductID = new List<int>();
            var listGioHang = GioHang;
            return View(GioHang);
        }
    }
}
