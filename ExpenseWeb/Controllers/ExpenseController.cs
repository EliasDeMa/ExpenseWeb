using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExpenseWeb.Database;
using ExpenseWeb.Domain;
using ExpenseWeb.Models;
using ExpenseWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseWeb.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IExpenseDatabase _expenseDatabase;
        private readonly IPhotoService _photoService;

        public ExpenseController(IExpenseDatabase expenseDatabase, IPhotoService photoService)
        {
            _expenseDatabase = expenseDatabase;
            _photoService = photoService;
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
                Amount = vm.Amount,
                Category = vm.Category
            };

            if (vm.File != null)
            {
                expense.PhotoPath = _photoService.AddPhoto(vm.File);
            }

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
                Category = expense.Category,
                PhotoPath = expense.PhotoPath
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
                Category = expense.Category
            };

            if (!string.IsNullOrEmpty(expense.PhotoPath))
            {
                expenseEdit.FilePath = expense.PhotoPath;
            }

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

            var origExpense = _expenseDatabase.GetExpense(id);
            var expense = new Expense
            {
                Description = vm.Description,
                Date = vm.Date,
                Amount = vm.Amount,
                Category = vm.Category
            };

            if (vm.File != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.File.FileName);
                
                if (!string.IsNullOrEmpty(origExpense.PhotoPath))
                {
                    _photoService.DeletePhoto(origExpense.PhotoPath);
                }

                expense.PhotoPath = _photoService.AddPhoto(vm.File);
            }
            else
            {
                if (!string.IsNullOrEmpty(origExpense.PhotoPath))
                    expense.PhotoPath = origExpense.PhotoPath;
            }

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
            var expense = _expenseDatabase.GetExpense(id);
            _photoService.DeletePhoto(expense.PhotoPath);
            _expenseDatabase.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
