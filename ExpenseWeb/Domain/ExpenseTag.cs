using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Domain
{
    public class ExpenseTag
    {
        public int ExpenseId { get; set; }
        public Expense Expense { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
