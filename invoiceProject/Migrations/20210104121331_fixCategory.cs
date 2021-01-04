using Microsoft.EntityFrameworkCore.Migrations;

namespace invoiceProject.Migrations
{
    public partial class fixCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoice_CategoryID",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Credit_CategoryID",
                table: "Credit");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CategoryID",
                table: "Invoice",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Credit_CategoryID",
                table: "Credit",
                column: "CategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoice_CategoryID",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Credit_CategoryID",
                table: "Credit");

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
        }
    }
}
