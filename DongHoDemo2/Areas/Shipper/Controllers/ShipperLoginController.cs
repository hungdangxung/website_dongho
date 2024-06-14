using AspNetCoreHero.ToastNotification.Abstractions;
using DiChoSaiGon.Helpper;
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

namespace DongHoDemo2.Areas.Shipper.Controllers
{
    [Area("Shipper")]
    public class ShipperLoginController : Controller
    {
        private readonly DongHoDemoContext _context;
        public INotyfService _notiyfService { get; }
        public ShipperLoginController(DongHoDemoContext context, INotyfService notyfService)
        {
            _context = context;
            _notiyfService = notyfService;
        }

        [AllowAnonymous]
        [Route("login-shipper.html", Name = "LoginShipper")]
        public IActionResult LoginShipper(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("ShipperId");
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
        [Route("login-shipper.html", Name = "LoginShipper")]
        public async Task<IActionResult> LoginShipper(LoginVM account, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(account.UserName);
                    if (!isEmail)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View(account);
                    }
                    var shipper = _context.Shippers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == account.UserName);
                    if (shipper == null)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View();
                    }
                    string pass = account.Password.ToLower();
                    if (shipper.Password != pass)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View(account);
                    }
                    HttpContext.Session.SetString("ShipperId", shipper.ShipperId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("ShipperId");
                    //Identity
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, shipper.ShipperName),
                            new Claim("ShipperId", shipper.ShipperId.ToString())
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
                return View(account);
            }
            return View(account);
        }

        [HttpGet]
        [Route("logout-shipper.html", Name = "LogoutShipper")]
        public IActionResult LogoutShipper()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("ShipperId");
            _notiyfService.Success("Đăng xuất thành công");
            return RedirectToAction("LoginShipper", "ShipperLogin");
        }
    }
}