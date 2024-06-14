using System;
using System.Collections.Generic;

#nullable disable

namespace DongHoDemo2.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? OrderNumber { get; set; }
        public int? Quantity { get; set; }
        public int? Discount { get; set; }
        public int? Total { get; set; }
        public DateTime? ShipDate { get; set; }
        public int? CustomerId { get; set; }
        public int? Price { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? ColorId { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Color Color { get; set; }
    }
}
