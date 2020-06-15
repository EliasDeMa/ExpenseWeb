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
using Microsoft.EntityFrameworkCore;

namespace ExpenseWeb.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ExpenseDbContext _expenseDbContext;
        private readonly IPhotoService _photoService;

        public ExpenseController(ExpenseDbContext expenseDbContext, IPhotoService photoService)
        {
            _expenseDbContext = expenseDbContext;
            _photoService = photoService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _expenseDbContext.Expenses.ToListAsync();

            return View(list.Select(item => new ExpenseIndexViewModel
            {
                Id = item.Id,
                Date = item.Date
            }).OrderByDescending(item => item.Date).ToList());
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
        public async Task<IActionResult> Create(ExpenseCreateViewModel vm)
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

            _ = await _expenseDbContext.AddAsync(expense);
            _ = await _expenseDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            var expense = await _expenseDbContext.FindAsync<Expense>(id);

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

        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _expenseDbContext.FindAsync<Expense>(id);

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
        public async Task<IActionResult> Edit(int id, ExpenseEditViewModel vm)
        {
            if (!TryValidateModel(vm))
            {
                return View(vm);
            }

            var origExpense = await _expenseDbContext.FindAsync<Expense>(id);


            origExpense.Description = vm.Description;
            origExpense.Date = vm.Date;
            origExpense.Amount = vm.Amount;
            origExpense.Category = vm.Category;


            if (vm.File != null)
            {
                if (!string.IsNullOrEmpty(origExpense.PhotoPath))
                {
                    _photoService.DeletePhoto(origExpense.PhotoPath);
                }

                origExpense.PhotoPath = _photoService.AddPhoto(vm.File);
            }

            _ = await _expenseDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { Id = id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var expense = await _expenseDbContext.FindAsync<Expense>(id);

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
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var expense = await _expenseDbContext.FindAsync<Expense>(id);

            if (!string.IsNullOrEmpty(expense.PhotoPath))
            {
                _photoService.DeletePhoto(expense.PhotoPath);
            }

            _expenseDbContext.Remove<Expense>(expense);
            _ = await _expenseDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
