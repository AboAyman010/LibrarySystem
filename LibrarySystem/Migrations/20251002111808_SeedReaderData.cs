using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class SeedReaderData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Readers (Name, Email)
                VALUES 
                ('Ahmed Ali', 'ahmed.ali@example.com'),
                ('Sara Mohamed', 'sara.mohamed@example.com'),
                ('Omar Hassan', 'omar.hassan@example.com'),
                ('Nora Adel', 'nora.adel@example.com'),
                ('Khaled Mostafa', 'khaled.mostafa@example.com');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Readers");

        }
    }
}
