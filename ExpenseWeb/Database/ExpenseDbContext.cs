using ExpenseWeb.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Database
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Description = "Groceries",
                },
                new Category
                {
                    Id = 2,
                    Description = "Shopping",
                },
                new Category
                {
                    Id = 3,
                    Description = "Barber",
                },
                new Category
                {
                    Id = 4,
                    Description = "Food",
                });

            modelBuilder.Entity<ExpenseTag>()
                .HasKey(et => new { et.ExpenseId, et.TagId });

            modelBuilder.Entity<ExpenseTag>()
                .HasOne(et => et.Expense)
                .WithMany(e => e.ExpenseTags)
                .HasForeignKey(et => et.ExpenseId);

            modelBuilder.Entity<ExpenseTag>()
                .HasOne(et => et.Tag)
                .WithMany(e => e.ExpenseTags)
                .HasForeignKey(et => et.TagId);

            modelBuilder.Entity<Tag>()
                .HasData(
                new Tag { Id = 1, Name= "Expensive"},
                new Tag { Id = 2, Name = "Cheap" },
                new Tag { Id = 3, Name = "Essential" },
                new Tag { Id = 4, Name = "Non-essential" }
                );
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ExpenseTag> ExpenseTags { get; set; }
    }
}
