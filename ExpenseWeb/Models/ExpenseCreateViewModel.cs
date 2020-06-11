using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Models
{
    public class ExpenseCreateViewModel
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount has to be positive.")]
        public decimal Amount { get; set; }
    }
}
