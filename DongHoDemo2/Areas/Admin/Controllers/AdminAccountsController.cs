using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DongHoDemo2.Models;
using Microsoft.AspNetCore.Authorization;
using DongHoDemo2.ModelViews;
using DiChoSaiGon.Helpper;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DongHoDemo2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly DongHoDemoContext _context;
        public INotyfService _notiyfService { get; }
        public AdminAccountsController(DongHoDemoContext context, INotyfService notyfService)
        {
            _context = context;
            _notiyfService = notyfService;
        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index()
        {
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleID", "Description");
            var dongHoDemoContext = _context.Accounts.Include(a => a.Role);
            return View(await dongHoDemoContext.ToListAsync());
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Description");
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleID", "Description", account.RoleId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
            return View(account);
        }

        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            //ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleID", "Description", account.RoleId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
            return View(account);
        }

        // POST: Admin/AdminAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleID", "Description", account.RoleId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
            return View(account);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
        [AllowAnonymous]
        [Route("login-admin.html", Name = "LoginAdmin")]
        public IActionResult LoginAdmin(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
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
        [Route("login-admin.html", Name = "LoginAdmin")]
        public async Task<IActionResult> LoginAdmin(LoginVM account, string returnUrl = null)
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
                    var admin = _context.Accounts
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == account.UserName && x.RoleId == 1);
                    if (admin == null)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View();
                    }
                    string pass = account.Password.ToLower();
                    if (admin.Password != pass)
                    {
                        _notiyfService.Error("Sai thông tin đăng nhập");
                        return View(account);
                    }
                    //kiem tra
                    if (admin.Active == false)
                    {
                        return RedirectToAction("ThongBao", "AdminAccounts");
                    }

                    //Luu session khachhang
                    HttpContext.Session.SetString("AccountId", admin.AccountId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("AccountId");
                    //Identity
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, admin.FullName),
                            new Claim("AccountId", admin.AccountId.ToString())
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
        [Route("logout-admin.html", Name = "LogoutAdmin")]
        public IActionResult LogoutAdmin()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccountId");
            _notiyfService.Success("Đăng xuất thành công");
            return RedirectToAction("LoginAdmin", "AdminAccounts");
        }
    }
}
