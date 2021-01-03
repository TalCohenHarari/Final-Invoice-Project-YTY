using Microsoft.EntityFrameworkCore.Migrations;

namespace invoiceProject.Migrations
{
    public partial class category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CategoryID",
                table: "Invoice",
                column: "CategoryID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Credit_CategoryID",
                table: "Credit",
                column: "CategoryID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Credit_Category_CategoryID",
                table: "Credit",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Category_CategoryID",
                table: "Invoice",
                column: "CategoryID",
                principalTable: "Category",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credit_Category_CategoryID",
                table: "Credit");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Category_CategoryID",
                table: "Invoice");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_CategoryID",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Credit_CategoryID",
                table: "Credit");
        }
    }
}
