using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseWeb.Migrations
{
    public partial class AddExpenseAppUserFKToExpense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExpenseAppUserId",
                table: "Expenses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseAppUserId",
                table: "Expenses",
                column: "ExpenseAppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_ExpenseAppUserId",
                table: "Expenses",
                column: "ExpenseAppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_ExpenseAppUserId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseAppUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseAppUserId",
                table: "Expenses");
        }
    }
}
