using AspNetCoreHero.ToastNotification.Abstractions;
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
    public class DonHangController : Controller
    {
        private readonly DongHoDemoContext _context;
        public INotyfService _notyfService { get; }
        public DonHangController(DongHoDemoContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpPost]
        //[Route("OrderDetail-{id}.html", Name = "OrderDetails")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Accounts");
                var khachhang = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));

                if (khachhang == null) return NotFound();
                var donhang = await _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.Shipper)
                .Include(x => x.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id && Convert.ToInt32(taikhoanID) == m.CustomerId);

                if (donhang == null) return NotFound();
                var chitietdonhang = _context.OrderDetails
                .Include(x => x.Product)
                .Include(x => x.Order)
                .Include(x => x.Color)
                .AsNoTracking()
                .Where(x => x.OrderId == id)
                .OrderBy(x => x.OrderDetailId)
                .ToList();
                XemDonHang donHang = new XemDonHang();
                donHang.DonHang = donhang;
                donHang.ChiTietDonHang = chitietdonhang;

                return PartialView("Details", donHang);
            }
            catch
            {
                return NotFound();
            }
        }

        //[HttpPost]
        public ActionResult DaNhanHang(int? id)
        {
            try
            {
                var order = _context.Orders
                    .Include(x => x.TransactStatus)
                    .AsNoTracking()
                    .SingleOrDefault(x => x.OrderId == id);
                if (order != null && order.TransactStatusId == 4)
                {
                    order.TransactStatusId = 1005;
                    _context.Entry(order).State = EntityState.Modified;
                    _context.SaveChanges();
                    _notyfService.Success("Xác nhận thành công");
                    return RedirectToAction("LichSuDatHang", "Accounts");
                }
                _notyfService.Error("Xác nhận không thành công");
                return RedirectToAction("LichSuDatHang", "Accounts");
            }
            catch
            {
                _notyfService.Error("Xác nhận không thành công");
                return RedirectToAction("LichSuDatHang", "Accounts");
            }
        }
    }
}
