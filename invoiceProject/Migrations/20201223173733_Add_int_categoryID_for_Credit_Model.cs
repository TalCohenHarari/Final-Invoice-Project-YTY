using Microsoft.EntityFrameworkCore.Migrations;

namespace invoiceProject.Migrations
{
    public partial class Add_int_categoryID_for_Credit_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Credit",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Credit");
        }
    }
}
