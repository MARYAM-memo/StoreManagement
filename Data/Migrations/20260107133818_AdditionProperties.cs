using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredDate",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedDate",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "OrderItems",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "OrderItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPurchaseDate",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Customers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalOrders",
                table: "Customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSpent",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveredDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastPurchaseDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TotalOrders",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "TotalSpent",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
