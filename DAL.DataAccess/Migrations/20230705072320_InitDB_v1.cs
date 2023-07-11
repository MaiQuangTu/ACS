using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.DataAccess.Migrations
{
    public partial class InitDB_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortenerURL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LongUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ShortUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TK_ShortenerURL", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortenerURL");
        }
    }
}
