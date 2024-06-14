using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DongHoDemo2.Models;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace DongHoDemo2.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly DongHoDemoContext _context;
        private readonly IConverter _converter;
        public AdminOrdersController(DongHoDemoContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        // GET: Admin/AdminOrders
        public async Task<IActionResult> Index()
        {
            var dongHoDemoContext = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .Include(o => o.TransactStatus);
            return View(await dongHoDemoContext.ToListAsync());
        }

        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var orderDetails = _context.OrderDetails
                .Include(x => x.Product)
                .Include(x => x.Color)
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

        // GET: Admin/AdminOrders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["ShipperID"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId");
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId");
            return View();
        }

        // POST: Admin/AdminOrders/Create
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

        // GET: Admin/AdminOrders/Edit/5
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
            ViewData["ShipperID"] = new SelectList(_context.Shippers, "ShipperId", "ShipperName", order.ShipperID);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,PaymentId,Note,ShipperID,Total,Address")] Order order)
        {
            var orderDetails = _context.OrderDetails
                .Include(o => o.Product)
                .AsNoTracking()
                .Where(o => o.OrderId == id)
                .ToList();
            ViewBag.listOrderDetails = orderDetails;
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
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
            ViewData["ShipperID"] = new SelectList(_context.Shippers, "ShipperId", "ShipperName", order.ShipperID);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Delete/5
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

        // POST: Admin/AdminOrders/Delete/5
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

        //In hoa don
        public ActionResult InHoaDon(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dh = _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Shipper)
                .Include(o => o.TransactStatus)
                .FirstOrDefault(m => m.OrderId == id);

            var ctdh = _context.OrderDetails
                        .AsNoTracking()
                        .Include(x => x.Product)
                        .Include(x => x.Color)
                        .Include(x => x.Order)
                        .Where(x => x.OrderId == dh.OrderId)
                        .OrderBy(x => x.ProductId)
                        .ToList();
            if (dh == null)
            {
                return NotFound();
            }
            // Get the HTML content of the invoice
            string html1 = $"";
            html1 += $"<html><body style=\"width: 500px; margin: 0 auto;\"><h1 style=\"text-align: center\">HÓA ĐƠN BÁN HÀNG</h1>" +
                $"<h4 style=\"text-align: center\">MÃ ĐƠN HÀNG: #{dh.OrderCode}</h4>" +
                //$"<img style=\"margin-left: 190px\" width=\"120px\" height=\"50px\" src=\"/images/mavach.png\">" +
                $"<h4>Khách hàng: {dh.Customer.FullName} - {dh.Customer.Phone}</h4>" +
                $"<h4>Ngày đặt: {dh.OrderDate}</h4>" +
                $"<h4>Địa chỉ: {dh.Customer.Address}</h4>" +
                $"<h4>Hình thức thanh toán: Thanh toán khi nhận hàng</h4>" +
                /*$"<h4>Trạng thái: {dh.TransactStatus.Status}</h4>" +*/
                $"<table style=\"border-collapse: collapse\" border=\"1\" width=\"550px\">" +
                $"<tr>" +
                    $"<th>STT</th>" +
                    $"<th>Sản phẩm</th>" +
                    $"<th>Số lượng</th>" +
                    $"<th>Màu sắc</th>" +
                    $"<th>Đơn giá</th>" +
                    $"<th>Thành tiền</th>" +
                 $"</tr>";

            string html2 = "";
            int i = 1;

            foreach (var item in ctdh)
            {
                var price = "";
                if (item.Product.Discount != null)
                {
                    price = item.Product.Discount.Value.ToString("#,##0");
                }
                else
                {
                    price = item.Product.Price.Value.ToString("#,##0");
                }
                    html2 +=
                    $"<tr>" +
                        $"<td>{i}</td>" +
                        $"<td>{item.Product.ProductName}</td>" +
                        $"<td>{item.Quantity}</td>" +
                        $"<td>{item.Color.ColorName}</td>" +
                        $"<td>{price} VNĐ</td>" +
                        $"<td>{item.Total.Value.ToString("#,##0")} VNĐ</td>" +
                     $"</tr>";
                i++;
            }

            string html3 = "";
            html3 +=
                /*"<tr>" +
                    "<td colspan=\"4\">Tạm tính</td>" +
                    $"<td>{ctdh.Sum(x => x.Total)}</td>" +
                "</tr>" +
                "<tr>" +
                    "<td colspan=\"4\">Giảm giá</td>" +
                    $"<td>{dh.GiamGia}</td>" +
                "</tr>" +
                "<tr>" +
                    "<td colspan=\"4\">Phí giao hàng</td>" +
                   $" <td>0</td>" +
                "</tr>" +*/
                "<tr>" +
                    $"<th colspan=\"6\">Tổng tiền: {dh.Total.Value.ToString("#,##0")} VNĐ</th>" +
                "</tr>" +
                $"</table>" +
                $"" +
                $"</body></html>";

            string htmlContent = html1 + html2 + html3;

            // Convert HTML to PDF
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait
                /*DocumentTitle = "PDF Report",
                Out = @"D:\NAM4\Đồ án\Project\Demo4\DongHoDemo2\PDFCreator\" + dh.OrderCode + ".pdf"*/
            },
                Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
            };

            var pdfBytes = _converter.Convert(doc);

            // Set the response content type and headers
            Response.ContentType = "application/pdf";
            Response.Headers.Add("content-disposition", "attachment;filename=donhang" + dh.OrderCode + ".pdf");

            // Write the PDF to the response
            return File(pdfBytes, "application/pdf");
        }
    }
}
