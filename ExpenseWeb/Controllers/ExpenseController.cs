using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ExpenseWeb.Database;
using ExpenseWeb.Domain;
using ExpenseWeb.Models;
using ExpenseWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExpenseWeb.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ExpenseDbContext _expenseDbContext;
        private readonly IPhotoService _photoService;
        private readonly ILogger<ExpenseController> _logger;

        public ExpenseController(ExpenseDbContext expenseDbContext, IPhotoService photoService,
            ILogger<ExpenseController> logger)
        {
            _expenseDbContext = expenseDbContext;
            _photoService = photoService;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _expenseDbContext.Expenses
                .Where(x => x.ExpenseAppUserId == userId)
                .ToListAsync();


            return View(list.Select(item => new ExpenseIndexViewModel
            {
                Id = item.Id,
                Date = item.Date
            }).OrderByDescending(item => item.Date).ToList());
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var categories = await _expenseDbContext.Categories.ToListAsync();
            var tags = await _expenseDbContext.Tags.ToListAsync();

            var expense = new ExpenseCreateViewModel
            {
                Date = DateTime.Now,
                Categories = categories.Select(item =>
                    new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Description,
                    }),
                Tags = tags.Select(item =>
                    new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name,
                    })
            };

            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(ExpenseCreateViewModel vm)
        {
            if (!TryValidateModel(vm))
            {
                return View(vm);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = new Expense
            {
                Description = vm.Description,
                Date = vm.Date,
                Amount = vm.Amount,
                CategoryId = vm.SelectedCategory,
                ExpenseTags = vm.SelectedTags.Select(id => new ExpenseTag { TagId = id }).ToList(),
                ExpenseAppUserId = userId
            };

            if (vm.File != null)
            {
                expense.PhotoPath = _photoService.AddPhoto(vm.File);
            }

            await _expenseDbContext.AddAsync(expense);
            await _expenseDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Detail(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = await _expenseDbContext.Expenses
                .Include(x => x.Category)
                .Include(x => x.ExpenseTags)
                .ThenInclude(et => et.Tag)
                .FirstOrDefaultAsync(expense => expense.Id == id && expense.ExpenseAppUserId == userId);

            var expenseDetail = new ExpenseDetailViewModel
            {
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                Category = expense.Category.Description,
                PhotoPath = expense.PhotoPath,
                Tags = expense.ExpenseTags.Select(item => item.Tag.Name)
            };

            return View(expenseDetail);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = await _expenseDbContext.Expenses
                .Include(x => x.ExpenseTags)
                .FirstOrDefaultAsync(x => x.Id == id && x.ExpenseAppUserId == userId);
            var categories = await _expenseDbContext.Categories.ToListAsync();
            var tags = await _expenseDbContext.Tags.ToListAsync();

            var expenseEdit = new ExpenseEditViewModel
            {
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                SelectedCategory = expense.Category.Id,
                Categories = categories.Select(item =>
                    new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Description,
                    }),
                Tags = tags.Select(item =>
                    new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name,
                        Selected = expense.ExpenseTags.Where(i => i.TagId == item.Id).Any()
                    })
            };

            if (!string.IsNullOrEmpty(expense.PhotoPath))
            {
                expenseEdit.FilePath = expense.PhotoPath;
            }

            return View(expenseEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ExpenseEditViewModel vm)
        {
            if (!TryValidateModel(vm))
            {
                return View(vm);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var origExpense = await _expenseDbContext.Expenses
                .Include(x => x.ExpenseTags)
                .FirstOrDefaultAsync(x => x.Id == id && x.ExpenseAppUserId == userId);

            _expenseDbContext.ExpenseTags.RemoveRange(origExpense.ExpenseTags);

            origExpense.Description = vm.Description;
            origExpense.Date = vm.Date;
            origExpense.Amount = vm.Amount;
            origExpense.CategoryId = vm.SelectedCategory;
            origExpense.ExpenseTags = vm.SelectedTags.Select(id => new ExpenseTag { TagId = id }).ToList();

            if (vm.File != null)
            {
                if (!string.IsNullOrEmpty(origExpense.PhotoPath))
                {
                    _photoService.DeletePhoto(origExpense.PhotoPath);
                }

                origExpense.PhotoPath = _photoService.AddPhoto(vm.File);
            }

            await _expenseDbContext.SaveChangesAsync();

            return RedirectToAction("Detail", new { Id = id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = await _expenseDbContext.Expenses
                .FirstOrDefaultAsync(x => x.Id == id && x.ExpenseAppUserId == userId);

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
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expense = await _expenseDbContext.Expenses
                .FirstOrDefaultAsync(x => x.Id == id && x.ExpenseAppUserId == userId);

            if (!string.IsNullOrEmpty(expense.PhotoPath))
            {
                _photoService.DeletePhoto(expense.PhotoPath);
            }

            _expenseDbContext.Expenses.Remove(expense);
            await _expenseDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
