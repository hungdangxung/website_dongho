using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.ModelViews
{
    public class SearchProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? Price { get; set; }
        public string Thumb { get; set; }
        public string Alias { get; set; }
    }
}
