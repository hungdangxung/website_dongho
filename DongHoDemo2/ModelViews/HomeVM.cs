using DongHoDemo2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DongHoDemo2.ModelViews
{
    public class HomeVM
    {
        public List<ProductHomeVM> products { get; set; } 
        public List<CategoryHomeVM> categories { get; set; }
    }
}
