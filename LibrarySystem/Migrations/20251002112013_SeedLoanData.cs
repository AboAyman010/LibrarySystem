using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedLoanData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Loans (BookId, ReaderId, LoanDate, ReturnDate)
                VALUES
                (1, 1, GETDATE(), NULL), -- Ahmed Ali استعار Clean Code ولسه مرجعهوش
                (2, 2, GETDATE(), NULL), -- Sara Mohamed استعار The Pragmatic Programmer ولسه مرجعهوش
                (3, 3, GETDATE(), DATEADD(DAY, 7, GETDATE())), -- Omar Hassan استعار Design Patterns ورجعه بعد 7 أيام
                (4, 4, GETDATE(), NULL), -- Nora Adel استعار Refactoring ولسه مرجعهوش
                (5, 5, GETDATE(), NULL); -- Khaled Mostafa استعار Introduction to Algorithms ولسه مرجعهوش
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Loans");

        }
    }
}
