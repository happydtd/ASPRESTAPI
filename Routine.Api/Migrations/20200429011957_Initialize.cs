using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.Api.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Country = table.Column<string>(maxLength: 50, nullable: true),
                    Industry = table.Column<string>(maxLength: 50, nullable: true),
                    Product = table.Column<string>(maxLength: 100, nullable: true),
                    Introduction = table.Column<string>(maxLength: 500, nullable: true),
                    BankruptTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    EmployeeNo = table.Column<string>(maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "BankruptTime", "Country", "Industry", "Introduction", "Name", "Product" },
                values: new object[] { new Guid("9b271f3c-4798-423d-aca5-990efad0c27a"), null, "USA", "Software", "Great Company", "Microsoft", "Software" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "BankruptTime", "Country", "Industry", "Introduction", "Name", "Product" },
                values: new object[] { new Guid("13f09e4a-7c60-4364-b291-9d67bc9d2f54"), null, "USA", "Internet", "Don't be evil", "Google", "Software" });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "BankruptTime", "Country", "Industry", "Introduction", "Name", "Product" },
                values: new object[] { new Guid("c84add17-1cf5-436c-995b-a4b21505aca1"), null, "China", "Internet", "Fubao Company", "Alipapa", "Software" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("833c57c1-81d2-45f3-a88a-63acf996cbcc"), new Guid("9b271f3c-4798-423d-aca5-990efad0c27a"), new DateTime(1982, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G003", "Mary", 1, "King" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("1585d3f4-e44d-40a7-a82b-40f540b06438"), new Guid("9b271f3c-4798-423d-aca5-990efad0c27a"), new DateTime(1977, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "G004", "Daniel", 1, "Brawn" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("c795686c-fec3-47aa-bfc5-1a117102e695"), new Guid("13f09e4a-7c60-4364-b291-9d67bc9d2f54"), new DateTime(1982, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G005", "Lady", 1, "GaGa" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("bfccfddf-8806-4b2c-86ee-f6e3e27da865"), new Guid("13f09e4a-7c60-4364-b291-9d67bc9d2f54"), new DateTime(1977, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "G006", "Michale", 1, "Jeke" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("b2b0e59c-a07d-4126-b91d-14cf11fd1fe4"), new Guid("c84add17-1cf5-436c-995b-a4b21505aca1"), new DateTime(1982, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "G007", "Daniel", 1, "Lin" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CompanyId", "DateOfBirth", "EmployeeNo", "FirstName", "Gender", "LastName" },
                values: new object[] { new Guid("ed7ad799-c641-4777-89ac-be61b47a7040"), new Guid("c84add17-1cf5-436c-995b-a4b21505aca1"), new DateTime(1977, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "G008", "Mark", 0, "Delen" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyId",
                table: "Employees",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
