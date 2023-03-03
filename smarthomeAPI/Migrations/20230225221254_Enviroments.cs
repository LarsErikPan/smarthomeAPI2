using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smarthomeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Enviroments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.CreateTable(
                name: "Enviroments",
                columns: table => new
                {
                    EnviromentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParentEnviromentID = table.Column<int>(type: "int", nullable: false),
                    EnviromentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enviroments", x => x.EnviromentId);
                    table.ForeignKey(
                        name: "FK_Enviroments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RawDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnviromentID = table.Column<int>(type: "int", nullable: false),
                    UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoggedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    accelerometer_x = table.Column<float>(type: "real", nullable: false),
                    accelerometer_y = table.Column<float>(type: "real", nullable: false),
                    accelerometer_z = table.Column<float>(type: "real", nullable: false),
                    gyroscope_x = table.Column<float>(type: "real", nullable: false),
                    gyroscope_y = table.Column<float>(type: "real", nullable: false),
                    gyroscope_z = table.Column<float>(type: "real", nullable: false),
                    compass_x = table.Column<float>(type: "real", nullable: false),
                    compass_y = table.Column<float>(type: "real", nullable: false),
                    compass_z = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RawDatas_Enviroments_EnviromentID",
                        column: x => x.EnviromentID,
                        principalTable: "Enviroments",
                        principalColumn: "EnviromentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enviroments_UserId_ParentEnviromentID",
                table: "Enviroments",
                columns: new[] { "UserId", "ParentEnviromentID" });

            migrationBuilder.CreateIndex(
                name: "IX_RawDatas_EnviromentID_LoggedTime",
                table: "RawDatas",
                columns: new[] { "EnviromentID", "LoggedTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RawDatas");

            migrationBuilder.DropTable(
                name: "Enviroments");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");
        }
    }
}
