﻿using ExpenseWeb.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Required(AllowEmptyStrings = false, ErrorMessage = "Expense must have a date")]
        public DateTime Date { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Expense must have an amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount has to be positive.")]
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }

        public IEnumerable<ExpenseCategory> Categories { get; } = Enum.GetValues(typeof(ExpenseCategory)).Cast<ExpenseCategory>();
        public IFormFile File { get; set; }
    }
}
