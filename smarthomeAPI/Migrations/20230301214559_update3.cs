using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smarthomeAPI.Migrations
{
    /// <inheritdoc />
    public partial class update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RawDatas_EnviromentID_LoggedTime",
                table: "RawDatas");

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                table: "RawDatas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "EnviromentName",
                table: "Enviroments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RawDatas_EnviromentID_LoggedTime_DeviceName",
                table: "RawDatas",
                columns: new[] { "EnviromentID", "LoggedTime", "DeviceName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RawDatas_EnviromentID_LoggedTime_DeviceName",
                table: "RawDatas");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                table: "RawDatas");

            migrationBuilder.AlterColumn<string>(
                name: "EnviromentName",
                table: "Enviroments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_RawDatas_EnviromentID_LoggedTime",
                table: "RawDatas",
                columns: new[] { "EnviromentID", "LoggedTime" });
        }
    }
}
