using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseWeb.Database;
using ExpenseWeb.Domain;
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

        public IActionResult Create()
        {
            var expense = new ExpenseCreateViewModel
            {
                Date = DateTime.Now,
            };

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExpenseCreateViewModel vm)
        {
            var expense = new Expense
            {
                Description = vm.Description,
                Date = vm.Date,
                Amount = vm.Amount
            };

            _expenseDatabase.Insert(expense);

            return RedirectToAction("Index");
        }
    }
}
