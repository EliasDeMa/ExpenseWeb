using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseWeb.Database;
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
            return View();
        }
    }
}
