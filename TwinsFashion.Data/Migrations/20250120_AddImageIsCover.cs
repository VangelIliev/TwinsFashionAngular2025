using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwinsFashion.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageIsCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsCover with default false
            migrationBuilder.AddColumn<bool>(
                name: "IsCover",
                table: "Images",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Make sure everything is false (explicit) in case of existing data
            migrationBuilder.Sql("UPDATE [Images] SET [IsCover] = 0;");

            // Unique index: allow only one cover image per product
            migrationBuilder.CreateIndex(
                name: "IX_Images_ProductId_IsCover",
                table: "Images",
                columns: new[] { "ProductId", "IsCover" },
                unique: true,
                filter: "[IsCover] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Images_ProductId_IsCover",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "IsCover",
                table: "Images");
        }
    }
}
