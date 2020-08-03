using Microsoft.EntityFrameworkCore.Migrations;

namespace BT_ReceiveDataMISA.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tbl_CauHinhDongBo",
                columns: table => new
                {
                    Token = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Id = table.Column<int>(nullable: false),
                    ChuKyThucHien = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbl_CauHinhDongBo", x => x.Token);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tbl_CauHinhDongBo");
        }
    }
}
