using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mycelium.Api.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Processor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstalledRam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GraphicsCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceSecurities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastScan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    LastAntivirusUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAntispywareUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsVirtualMachine = table.Column<bool>(type: "bit", nullable: false),
                    AntivirusEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RealTimeProtectionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    NisEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TamperProtectionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AntispywareEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DomainFirewallEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PrivateFirewallEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PublicFirewallEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSecurities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Hash = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceDetailsId = table.Column<int>(type: "int", nullable: false),
                    DeviceSecurityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_DeviceDetails_DeviceDetailsId",
                        column: x => x.DeviceDetailsId,
                        principalTable: "DeviceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_DeviceSecurities_DeviceSecurityId",
                        column: x => x.DeviceSecurityId,
                        principalTable: "DeviceSecurities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devices_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    AuthenticityToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwoFactorToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastVerified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceDisks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOsDisk = table.Column<bool>(type: "bit", nullable: false),
                    Used = table.Column<double>(type: "float", nullable: false),
                    Size = table.Column<double>(type: "float", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceDisks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceDisks_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeviceSoftware",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceSoftware", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceSoftware_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceDisks_DeviceId",
                table: "DeviceDisks",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceDetailsId",
                table: "Devices",
                column: "DeviceDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceSecurityId",
                table: "Devices",
                column: "DeviceSecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_OrganisationId",
                table: "Devices",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceSoftware_DeviceId",
                table: "DeviceSoftware",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OrganisationId",
                table: "Users",
                column: "OrganisationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceDisks");

            migrationBuilder.DropTable(
                name: "DeviceSoftware");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceDetails");

            migrationBuilder.DropTable(
                name: "DeviceSecurities");

            migrationBuilder.DropTable(
                name: "Organisations");
        }
    }
}
