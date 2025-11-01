using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ovia.Migrations
{
    public partial class addnewtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5315), new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5371) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5380), new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5383) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5386), new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5402) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5410), new DateTime(2025, 10, 27, 13, 32, 32, 265, DateTimeKind.Local).AddTicks(5413) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4777), new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4823) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4831), new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4833) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4835), new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4844) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreationDate", "LastUpdateDate" },
                values: new object[] { new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4850), new DateTime(2025, 9, 29, 13, 14, 58, 775, DateTimeKind.Local).AddTicks(4852) });
        }
    }
}
