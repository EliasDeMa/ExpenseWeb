using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ExpenseWeb.Database;
using ExpenseWeb.Domain;
using ExpenseWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseWeb.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IExpenseDatabase _expenseDatabase;

        public StatisticsController(IExpenseDatabase expenseDatabase)
        {
            _expenseDatabase = expenseDatabase;
        }

        public IActionResult Index()
        {
            var expenses = _expenseDatabase.GetExpenses();
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

            var statistics = new StatisticsIndexViewModel
            {
                Highest = highestShow,
                Lowest = lowestShow,
                HighestDay = GetHighestDay(expenses),
                Monthly = MonthlyExpenses(expenses),
                HighestCategory = HighestCategoryExpense(expenses),
                LowestCategory = LowestCategoryExpense(expenses)
            };

            return View(statistics);
        }

        private (ExpenseCategory, decimal) HighestCategoryExpense(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(x => x.Category)
                .Select(x => (x.Key, x.AsEnumerable().Sum(x => x.Amount)))
                .OrderByDescending(x => x.Item2)
                .First();
        }

        private (ExpenseCategory, decimal) LowestCategoryExpense(IEnumerable<Expense> expenses)
        {
            return expenses.GroupBy(x => x.Category)
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
