using DongHoDemo2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.ModelViews
{
    public class CartItem
    {
        public Product product { get; set; }
        public int quantity { get; set; }
        public Color color { get; set; }
        public double total()
        {
            if (product.Discount != null)
            {
                return quantity * product.Discount.Value;
            }
            return quantity * product.Price.Value;
        }
    }
}
