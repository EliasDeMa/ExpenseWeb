using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ExpenseWeb.Database;
using ExpenseWeb.Domain;
using ExpenseWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseWeb.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ExpenseDbContext _expenseDbContext;

        public StatisticsController(ExpenseDbContext expenseDbContext)
        {
            _expenseDbContext = expenseDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _expenseDbContext.Expenses.ToListAsync();
            StatisticsExpenseModel highestShow = null;
            StatisticsExpenseModel lowestShow = null;

            Expense highest = expenses.OrderByDescending(x => x.Amount).FirstOrDefault();

            Expense lowest = expenses.OrderBy(x => x.Amount).FirstOrDefault();

            if (highest != null)
            {
                highestShow = new StatisticsExpenseModel
                {
                    Amount = highest.Amount,
                    Description = highest.Description,
                    Date = highest.Date
                };
            }

            if (lowest != null)
            {
                lowestShow = new StatisticsExpenseModel
                {
                    Amount = lowest.Amount,
                    Description = lowest.Description,
                    Date = lowest.Date
                };
            }

            var statistics = new StatisticsIndexViewModel();

            if (expenses.Count > 0)
            {
                var (highestCategoryId, highestCategoryExpense) = HighestCategoryExpense(expenses);
                var highestCategoryItem = await _expenseDbContext.Categories.FindAsync(highestCategoryId);

                var (lowestCategoryId, lowestCategoryExpense) = LowestCategoryExpense(expenses);
                var lowestCategoryItem = await _expenseDbContext.Categories.FindAsync(lowestCategoryId);

                statistics.Highest = highestShow;
                statistics.Lowest = lowestShow;
                statistics.HighestDay = GetHighestDay(expenses);
                statistics.Monthly = MonthlyExpenses(expenses);
                statistics.HighestCategory = (highestCategoryItem.Description, highestCategoryExpense);
                statistics.LowestCategory = (lowestCategoryItem.Description, lowestCategoryExpense);
            }

            return View(statistics);
        }

       private (int, decimal) HighestCategoryExpense(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(x => x.CategoryId)
                .Select(x => (x.Key, x.AsEnumerable().Sum(x => x.Amount)))
                .OrderByDescending(x => x.Item2)
                .First();
        }

       private (int, decimal) LowestCategoryExpense(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(x => x.CategoryId)
                .Select(x => (x.Key, x.AsEnumerable().Sum(x => x.Amount)))
                .OrderBy(x => x.Item2)
                .First();
        }


        private List<((int, int), decimal)> MonthlyExpenses(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(x => (x.Date.Year, x.Date.Month))
                .Select(item => (item.Key, item.AsEnumerable().Sum(x => x.Amount)))
                .OrderByDescending(item => item.Key.Year)
                .ThenByDescending(x => x.Key.Month)
                .ToList();
        } 

        private (DateTime?, decimal) GetHighestDay(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(x => x.Date.Date)
                .Select(x => (x.Key, x.AsEnumerable().Sum(x => x.Amount)))
                .OrderByDescending(x => x.Item2)
                .First();
        }
    }
}
