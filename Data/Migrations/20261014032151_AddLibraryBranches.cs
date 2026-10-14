using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryBranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LibraryBranchId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LibraryBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BranchCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryBranches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_LibraryBranchId",
                table: "Items",
                column: "LibraryBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_LibraryBranches_LibraryBranchId",
                table: "Items",
                column: "LibraryBranchId",
                principalTable: "LibraryBranches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_LibraryBranches_LibraryBranchId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "LibraryBranches");

            migrationBuilder.DropIndex(
                name: "IX_Items_LibraryBranchId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "LibraryBranchId",
                table: "Items");
        }
    }
}
