using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Models
{
    public class StatisticsIndexViewModel
    {
        public decimal Highest { get; set; }
        public decimal Lowest { get; set; }
        public (DateTime?, decimal) HighestDay { get; set; }
    }
}
