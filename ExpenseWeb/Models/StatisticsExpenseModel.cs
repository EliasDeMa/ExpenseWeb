﻿using ExpenseWeb.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Models
{
    public class StatisticsExpenseModel
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
    }
}
