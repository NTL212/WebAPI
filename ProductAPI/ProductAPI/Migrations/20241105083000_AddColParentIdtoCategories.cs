using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddColParentIdtoCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// Thêm cột ParentId vào bảng Categories
			migrationBuilder.AddColumn<int>(
				name: "ParentId",
				table: "Categories",
				type: "int",
				nullable: true);

			// Thiết lập khóa ngoại cho cột ParentId tham chiếu đến CategoryId trong bảng Categories
			migrationBuilder.AddForeignKey(
				name: "FK_Categories_ParentId",
				table: "Categories",
				column: "ParentId",
				principalTable: "Categories",
				principalColumn: "CategoryId",
				onDelete: ReferentialAction.Restrict);

		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
			// Xóa khóa ngoại của cột ParentId trong bảng Categories
			migrationBuilder.DropForeignKey(
				name: "FK_Categories_ParentId",
				table: "Categories");

			// Xóa cột ParentId trong bảng Categories
			migrationBuilder.DropColumn(
				name: "ParentId",
				table: "Categories");
		}
    }
}
