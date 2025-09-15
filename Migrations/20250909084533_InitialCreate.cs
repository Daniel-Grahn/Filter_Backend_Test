using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace filter_api_test.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Filters",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filters", x => new { x.SourceId, x.UserId, x.FieldName });
                    table.UniqueConstraint("AK_Filters_Id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredFilters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    IsPersonal = table.Column<bool>(type: "bit", nullable: false),
                    SourceId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filters = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFilters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Filters");

            migrationBuilder.DropTable(
                name: "StoredFilters");
        }
    }
}
