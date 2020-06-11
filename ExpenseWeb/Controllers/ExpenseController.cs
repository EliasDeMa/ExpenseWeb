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
                })
                .OrderBy(x => x.Date);

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
            if (!TryValidateModel(vm))
            {
                return View(vm);
            }

            var expense = new Expense
            {
                Description = vm.Description,
                Date = vm.Date,
                Amount = vm.Amount
            };

            _expenseDatabase.Insert(expense);

            return RedirectToAction("Index");
        }

        public IActionResult Detail(int id)
        {
            var expense = _expenseDatabase.GetExpense(id);

            var expenseDetail = new ExpenseDetailViewModel
            {
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
            };

            return View(expenseDetail);
        }

        public IActionResult Edit(int id)
        {
            var expense = _expenseDatabase.GetExpense(id);

            var expenseEdit = new ExpenseEditViewModel
            {
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
            };

            return View(expenseEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ExpenseEditViewModel vm)
        {
            if (!TryValidateModel(vm))
            {
                return View(vm);
            }

            var expense = new Expense
            {
                Description = vm.Description,
                Date = vm.Date,
                Amount = vm.Amount
            };

            _expenseDatabase.Update(id, expense);

            return RedirectToAction("Detail", new { Id = id });
        }

        public IActionResult Delete(int id)
        {
            var expense = _expenseDatabase.GetExpense(id);

            var expenseDelete = new ExpenseDeleteViewModel
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Date = expense.Date,
            };

            return View(expenseDelete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            _expenseDatabase.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
