using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace filter_api_test.Migrations
{
    /// <inheritdoc />
    public partial class RenamedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StoredFilters",
                table: "StoredFilters");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Filters_Id",
                table: "Filters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Filters",
                table: "Filters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FilterCompositions",
                table: "FilterCompositions");

            migrationBuilder.RenameTable(
                name: "StoredFilters",
                newName: "StoredFilter");

            migrationBuilder.RenameTable(
                name: "Filters",
                newName: "Filter");

            migrationBuilder.RenameTable(
                name: "FilterCompositions",
                newName: "FilterComposition");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoredFilter",
                table: "StoredFilter",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Filter_Id",
                table: "Filter",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Filter",
                table: "Filter",
                columns: new[] { "SourceId", "UserId", "FieldName" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FilterComposition",
                table: "FilterComposition",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StoredFilter",
                table: "StoredFilter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FilterComposition",
                table: "FilterComposition");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Filter_Id",
                table: "Filter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Filter",
                table: "Filter");

            migrationBuilder.RenameTable(
                name: "StoredFilter",
                newName: "StoredFilters");

            migrationBuilder.RenameTable(
                name: "FilterComposition",
                newName: "FilterCompositions");

            migrationBuilder.RenameTable(
                name: "Filter",
                newName: "Filters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoredFilters",
                table: "StoredFilters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FilterCompositions",
                table: "FilterCompositions",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Filters_Id",
                table: "Filters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Filters",
                table: "Filters",
                columns: new[] { "SourceId", "UserId", "FieldName" });
        }
    }
}
