using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Helpper;
using DongHoDemo2.Extensions;
using DongHoDemo2.Models;
using DongHoDemo2.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DongHoDemo2.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly DongHoDemoContext _context;
        public INotyfService _notiyfService { get; }
        public AccountsController(DongHoDemoContext context, INotyfService notyfService)
        {
            _context = context;
            _notiyfService = notyfService;
        }
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }

        [Route("dia-chi-giao-hang.html", Name = "AddressShip")]
        public IActionResult AddressShip()
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                    return Json(data: "Số điện thoại: " + Phone + " đã được sử dụng");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                    return Json(data: "Email: " + Email + " đã được sử dụng<br/>");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public IActionResult DangKyTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterVM taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var kh = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == taikhoan.Email);
                    /*var kh1 = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Phone.Trim() == taikhoan.Phone);*/
                    if (kh != null)
                    {
                        _notiyfService.Error("Email đã được sử dụng");
                        return View(taikhoan);
                    }
                    /*if (kh1 != null)
                    {
                        _notiyfService.Error("Số điện thoại đã được sử dụng");
                        return View(taikhoan);
                    }*/
                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FullName = taikhoan.FullName,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password.ToLower() + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt.Trim(),
                        CreateDate = DateTime.Now
                    };
                    try
                    {
                        ValidateEmail(khachhang.Email);
                        ValidatePhone(khachhang.Phone);
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Luu session khachhang
                        HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                        var taikhoanID = HttpContext.Session.GetString("CustomerId");
                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.FullName),
                            new Claim("CustomerId", khachhang.CustomerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        //Gio hang
                        //var ShoppingCart = GioHang;
                        _notiyfService.Success("Đăng ký thành công");
                        return RedirectToAction("Login", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("DangKyTaiKhoan", "Accounts");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }
        }
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            //Check gio hang neu da co thi tra ve
            //ViewBag.ShoppingCarts = GioHang;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginVM customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View(customer);
                    }
                    var khachhang = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == customer.UserName);
                    if (khachhang == null)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View();
                    }    
                    string pass = (customer.Password.ToLower() + khachhang.Salt.Trim()).ToMD5();
                    if (khachhang.Password != pass)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View(customer);
                    }
                    //kiem tra
                    if (khachhang.Active == false)
                    {
                        return RedirectToAction("ThongBao", "Accounts");
                    }

                    //Luu session khachhang
                    HttpContext.Session.SetString("CustomerId", khachhang.CustomerId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    //Identity
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachhang.FullName),
                            new Claim("CustomerId", khachhang.CustomerId.ToString())
                        };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    //Gio hang
                    //var ShoppingCart = GioHang;
                    _notiyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return View(customer);
            }
            return View(customer);
        }

        [HttpGet]
        [Route("dang-xuat.html", Name = "Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Login", "Accounts");
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("doi-mk.html", Name = "DoiMatKhau")]
        public IActionResult DoiMatKhau()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("doi-mk.html", Name = "DoiMatKhau")]
        public async Task<IActionResult> DoiMatKhau(ChangePasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
                    ViewBag.AllCategories = listCats;
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    if (taikhoanID == null)
                    {
                        return RedirectToAction("Login", "Accounts");
                    }
                    var khachhang = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                    if (khachhang == null)
                    {
                        return RedirectToAction("Login", "Accounts");
                    }
                    var pass = (model.PasswordNow.Trim().ToLower() + khachhang.Salt.Trim()).ToMD5();
                    if (pass == khachhang.Password)
                    {
                        string passnew = (model.Password.Trim().ToLower() + khachhang.Salt.Trim()).ToMD5();
                        khachhang.Password = passnew;
                        _context.Update(khachhang);
                        await _context.SaveChangesAsync();
                        _notiyfService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    _notiyfService.Error("Sai mật khẩu");
                    return View(model);
                }
            }
            catch
            {
                return View(model);
            }
            return View(model);
        }

        [AllowAnonymous]
        [Route("lich-su-dat-hang.html", Name = "LichSuDatHang")]
        public IActionResult LichSuDatHang()
        {
            var listCats = _context.Categories.Include(c => c.Products)
                .AsNoTracking()
                .Take(6)
                .ToList();
            ViewBag.AllCategories = listCats;
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    var listOrder = _context.Orders
                    .Include(x => x.Customer)
                    .Include(x => x.TransactStatus)
                    .AsNoTracking()
                    .Where(x => x.CustomerId == khachhang.CustomerId)
                    .OrderByDescending(x => x.OrderDate)
                    .ToList();
                    ViewBag.ListOrder = listOrder;
                    return View(khachhang);
                }
            }
            return RedirectToAction("Login", "Accounts");
        }
    }
}