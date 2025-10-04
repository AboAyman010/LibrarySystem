using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    UPDATE Books SET ImageUrl = '/images/1.png' WHERE Title = 'Clean Code';
    UPDATE Books SET ImageUrl = '/images/2.png' WHERE Title = 'The Pragmatic Programmer';
    UPDATE Books SET ImageUrl = '/images/3.png' WHERE Title = 'Design Patterns';
    UPDATE Books SET ImageUrl = '/images/4.png' WHERE Title = 'Refactoring';
    UPDATE Books SET ImageUrl = '/images/5.png' WHERE Title = 'Introduction to Algorithms';
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Books");
        }
    }
}
