using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TransactionsApi.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "transaction_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "Transaction_type_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "TransactionType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Ammount = table.Column<double>(nullable: false),
                    IsFraud = table.Column<bool>(nullable: false),
                    NameDest = table.Column<string>(maxLength: 50, nullable: false),
                    NameOrig = table.Column<string>(maxLength: 50, nullable: false),
                    NewBalanceDest = table.Column<double>(nullable: false),
                    NewBalanceOrig = table.Column<double>(nullable: false),
                    OldBalanceDest = table.Column<double>(nullable: false),
                    OldBalanceOrig = table.Column<double>(nullable: false),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    TransactionTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_TransactionType_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TransactionTypeId",
                table: "Transaction",
                column: "TransactionTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "TransactionType");

            migrationBuilder.DropSequence(
                name: "transaction_hilo");

            migrationBuilder.DropSequence(
                name: "Transaction_type_hilo");
        }
    }
}
