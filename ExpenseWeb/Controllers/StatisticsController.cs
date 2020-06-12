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

            if (highest != default)
            {
                highestShow = new StatisticsExpenseModel
                {
                    Amount = highest.Amount,
                    Description = highest.Description,
                    Date = highest.Date
                };
            }

            if (lowest != default)
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
                Monthly = MonthlyExpenses(expenses)
            };

            return View(statistics);
        }

        private List<((int, int), decimal)> MonthlyExpenses(IEnumerable<Expense> expenses)
        {
            var monthlyExpenses = new List<((int, int), decimal)>();
            var monthGroupings = expenses.GroupBy(x => (x.Date.Year, x.Date.Month));

            foreach (var item in monthGroupings)
            {
                decimal total = item.AsEnumerable().Sum(x => x.Amount);

                monthlyExpenses.Add((item.Key, total));
            }

            return monthlyExpenses;
        } 

        private (DateTime?, decimal) GetHighestDay(IEnumerable<Expense> expenses)
        {
            var dayGroupings = expenses.GroupBy(x => x.Date.Date);

            decimal total = 0;
            DateTime? highestDate = null;

            foreach (var date in dayGroupings)
            {
                decimal tempTotal = date.AsEnumerable().Sum(x => x.Amount);

                if (tempTotal > total)
                {
                    total = tempTotal;
                    highestDate = date.Key;
                }
            }

            return (highestDate, total);
        }
    }
}
