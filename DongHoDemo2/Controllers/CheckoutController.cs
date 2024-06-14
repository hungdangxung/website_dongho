using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Helpper;
using DongHoDemo2.Extensions;
using DongHoDemo2.Models;
using DongHoDemo2.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DongHoDemoContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutController(DongHoDemoContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
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
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            //lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if(taikhoanID != null)
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            /*ViewData["listTinhThanh"] = new SelectList(_context.Locations
                .Where(x => x.Levels == 1)
                .OrderBy(x => x.Type).ToList(), "Location", "Name");*/
            ViewBag.GioHang = cart;
            return View(model);
        }
        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            //Lay ra gio hang de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;

                khachhang.Address = muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("Login", "Accounts", new { returnUrl = "/checkout.html" });
            }
            try
            {
                if (ModelState.IsValid)
                {
                    //Khoi tao don hang
                    Order donhang = new Order();
                    donhang.CustomerId = model.CustomerId;
                    donhang.Address = model.Address;

                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 1;//Don hang moi
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    donhang.Note = Utilities.StripHTML(model.Note);
                    donhang.Total = Convert.ToInt32(cart.Sum(x => x.total()));
                    donhang.OrderCode = "DH" + GenerateRandomNumber(6);
                    _context.Add(donhang);
                    _context.SaveChanges();
                    //tao danh sach don hang
                    List<Product> listOrderProducts = new List<Product>();
                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = donhang.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Quantity = item.quantity;
                        orderDetail.Total = Convert.ToInt32(item.total());
                        /*orderDetail.Price = item.product.Price;*/
                        if(item.product.Discount != null)
                        {
                            orderDetail.Price = item.product.Discount;
                        }
                        else
                        {
                            orderDetail.Price = item.product.Price;
                        }
                        orderDetail.CreateDate = DateTime.Now;
                        orderDetail.ColorId = item.color.ColorId;

                        var orderProduct = _context.Products
                            .AsNoTracking()
                            .SingleOrDefault(x => x.ProductId == item.product.ProductId);
                        orderProduct.UnitslnStock = orderProduct.UnitslnStock - item.quantity;
                        _context.Update(orderProduct);

                        _context.Add(orderDetail);
                    }
                        _context.SaveChanges();
                    //clear gio hang
                    HttpContext.Session.Remove("GioHang");
                    //Xuat thong bao
                    _notyfService.Success("Đơn hàng đặt thành công");
                    //cap nhat thong tin khach hang
                    return RedirectToAction("Success");
                }
            }
            catch
            {
                //ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
                ViewBag.GioHang = cart;
                return View(model);
            }
            //ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }
        private static string GenerateRandomNumber(int length)
        {
            var random = new Random();
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = (char)('0' + random.Next(10));
            }
            return new string(result);
        }

        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders
                    .Where(x => x.CustomerId == Convert.ToInt32(taikhoanID))
                    .OrderByDescending(x => x.OrderDate)
                    .FirstOrDefault();
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.DonHangID = donhang.OrderId;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Address;
                return View(successVM);
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult HuyDonHang(int id)
        {
            try
            {
                var order = _context.Orders
                    .Include(x => x.TransactStatus)
                    .AsNoTracking()
                    .SingleOrDefault(x => x.OrderId == id);

                var orderDetails = _context.OrderDetails
                    .Include(x => x.Order)
                    .Include(x => x.Product)
                    .Include(x => x.Color)
                    .AsNoTracking()
                    .Where(x => x.OrderId == order.OrderId)
                    .ToList();
                Product product = new Product();
                if(order.TransactStatusId == 1)
                {
                    order.TransactStatusId = 5;
                    foreach (var item in orderDetails)
                    {
                        product = _context.Products
                        .Include(x => x.OrderDetails)
                        .AsNoTracking()
                        .FirstOrDefault(x => x.ProductId == item.ProductId);

                        product.UnitslnStock += item.Quantity;
                        _context.Update(product);
                    }
                    _context.Entry(order).State = EntityState.Modified;
                    _context.SaveChanges();
                    //_notyfService.Success("Hủy đơn hàng thành công");
                    return Json(new { success = true, message = "Hủy đơn hàng thành công" });
                }
                return Json(new { success = false, message = "Đơn hàng đã được hủy." });
            }
            catch
            {
                return Json(new { success = false, message = "Đơn hàng đã được hủy." });
            }
        }
    }
}
