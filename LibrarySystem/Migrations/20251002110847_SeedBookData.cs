using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Books ( Title, Author, TotalCopies, AvailableCopies)
                VALUES 
                ( 'Clean Code', 'Robert C. Martin', 10, 8),
                ( 'The Pragmatic Programmer', 'Andrew Hunt', 5, 5),
                ( 'Design Patterns', 'Erich Gamma', 7, 6),
                ( 'Refactoring', 'Martin Fowler', 4, 4),
                ( 'Introduction to Algorithms', 'Thomas H. Cormen', 6, 6);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Books");
        }
    }
}
