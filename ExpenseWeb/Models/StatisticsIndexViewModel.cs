using ExpenseWeb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Models
{
    public class StatisticsIndexViewModel
    {
        public StatisticsExpenseModel Highest { get; set; }
        public StatisticsExpenseModel Lowest { get; set; }
        public (DateTime?, decimal) HighestDay { get; set; }
        public List<((int, int), decimal)> Monthly { get; set; }
        public (ExpenseCategory, decimal) HighestCategory { get; set; }
    }
}
