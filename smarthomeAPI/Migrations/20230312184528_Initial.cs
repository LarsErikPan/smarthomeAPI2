using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smarthomeAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Environments",
                columns: table => new
                {
                    EnvironmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParentEnvironmentID = table.Column<int>(type: "int", nullable: false),
                    EnvironmentName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Environments", x => x.EnvironmentId);
                    table.ForeignKey(
                        name: "FK_Environments_Users_UserId",
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
                    EnvironmentID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoggedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_RawDatas_Environments_EnvironmentID",
                        column: x => x.EnvironmentID,
                        principalTable: "Environments",
                        principalColumn: "EnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Environments_UserId_ParentEnvironmentID",
                table: "Environments",
                columns: new[] { "UserId", "ParentEnvironmentID" });

            migrationBuilder.CreateIndex(
                name: "IX_RawDatas_EnvironmentID_LoggedTime_DeviceName",
                table: "RawDatas",
                columns: new[] { "EnvironmentID", "LoggedTime", "DeviceName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RawDatas");

            migrationBuilder.DropTable(
                name: "Environments");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
