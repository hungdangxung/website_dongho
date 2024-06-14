using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DongHoDemo2.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using DongHoDemo2.ModelViews;
using DiChoSaiGon.Helpper;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace DongHoDemo2.Areas.Shipper.Controllers
{
    [Area("Shipper")]
    public class HomeController : Controller
    {
        private readonly DongHoDemoContext _context;
        public INotyfService _notiyfService { get; }
        public HomeController(DongHoDemoContext context, INotyfService notyfService)
        {
            _context = context;
            _notiyfService = notyfService;
        }

        // GET: Shipper/Home
        [Route("Shipper")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var taikhoanID = HttpContext.Session.GetString("ShipperId");
            if (taikhoanID != null)
            {
                var shipper = _context.Shippers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.ShipperId == Convert.ToInt32(taikhoanID));
                if (shipper != null)
                {
                    var listOrders = _context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.Shipper)
                        .Include(o => o.TransactStatus)
                        .Where(o => o.ShipperID == shipper.ShipperId && o.TransactStatusId != 5)
                        .OrderByDescending(o => o.OrderDate);
                    return View(await listOrders.ToListAsync());
                }
            }
            return RedirectToAction("Login", "Accounts");

        }

        // GET: Shipper/Home/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var orderDetails = _context.OrderDetails
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(o => o.OrderId == id)
                .ToList();
            ViewBag.listOrderDetails = orderDetails;
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Shipper/Home/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["ShipperID"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId");
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId");
            return View();
        }

        // POST: Shipper/Home/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,PaymentId,Note,ShipperID,Total,Address")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["ShipperID"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipperID);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // GET: Shipper/Home/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var orderDetails = _context.OrderDetails
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(o => o.OrderId == id)
                .ToList();
            ViewBag.listOrderDetails = orderDetails;
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .Include(o => o.Shipper)
                .FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["ShipperID"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipperID);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // POST: Shipper/Home/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,PaymentId,Note,ShipperID,Total,Address")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (order != null)
                    {
                        if (order.TransactStatusId == 3)
                        {
                            order.ShipDate = DateTime.Now;
                        }
                        if (order.TransactStatusId == 5)
                        {
                            order.Deleted = true;
                        }
                    }
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    _notiyfService.Success("Cập nhật trạng thái thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["ShipperID"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipperID);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // GET: Shipper/Home/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Shipper/Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

    }
}
