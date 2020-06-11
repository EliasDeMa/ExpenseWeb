using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseWeb.Database;
using ExpenseWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseWeb.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseDatabase _expenseDatabase;

        public ExpenseController(IExpenseDatabase expenseDatabase)
        {
            _expenseDatabase = expenseDatabase;
        }

        public IActionResult Index()
        {
            var list = _expenseDatabase.GetExpenses()
                .Select(item => new ExpenseIndexViewModel
                {
                    Id = item.Id,
                    Date = item.Date
                });

            return View(list);
        }
    }
}
