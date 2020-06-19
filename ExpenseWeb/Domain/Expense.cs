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
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public string PhotoPath { get; set; }

        public ICollection<ExpenseTag> ExpenseTags { get; set; }
        public ExpenseAppUser ExpenseAppUser { get; set; }
        public string ExpenseAppUserId { get; set; }
    }
}