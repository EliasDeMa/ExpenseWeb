using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseWeb.Domain
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public string PhotoPath { get; set; }
    }

    public enum ExpenseCategory
    {
        Unassigned,
        Shopping,
        Barber,
        Groceries
    }
}