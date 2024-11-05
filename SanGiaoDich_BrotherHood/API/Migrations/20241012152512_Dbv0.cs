using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class Dbv0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "varchar(20)", nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(12)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(6)", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Introduce = table.Column<string>(type: "ntext", nullable: true),
                    ImageAccount = table.Column<string>(type: "varchar(150)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeBanned = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    IDCategory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameCate = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.IDCategory);
                });

            migrationBuilder.CreateTable(
                name: "AddressDetails",
                columns: table => new
                {
                    IDAddress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceCity = table.Column<string>(type: "Nvarchar(50)", nullable: false),
                    District = table.Column<string>(type: "Nvarchar(50)", nullable: false),
                    Wardcommune = table.Column<string>(type: "Nvarchar(50)", nullable: false),
                    AdditionalDetail = table.Column<string>(type: "Nvarchar(50)", nullable: false),
                    UserName = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressDetails", x => x.IDAddress);
                    table.ForeignKey(
                        name: "FK_AddressDetails_Accounts_UserName",
                        column: x => x.UserName,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    IDBill = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "varchar(20)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(12)", nullable: true),
                    Email = table.Column<string>(type: "varchar(150)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReceipt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(70)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UerName = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.IDBill);
                    table.ForeignKey(
                        name: "FK_Bills_Accounts_UerName",
                        column: x => x.UerName,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserSend = table.Column<string>(type: "varchar(20)", nullable: true),
                    UserReceive = table.Column<string>(type: "varchar(20)", nullable: true),
                    Content = table.Column<string>(type: "ntext", nullable: true),
                    TypeContent = table.Column<string>(type: "ntext", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    sendingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Accounts_UserReceive",
                        column: x => x.UserReceive,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Accounts_UserSend",
                        column: x => x.UserSend,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    IDProduct = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IDCategory = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "ntext", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    UserName = table.Column<string>(type: "varchar(20)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.IDProduct);
                    table.ForeignKey(
                        name: "FK_Products_Accounts_UserName",
                        column: x => x.UserName,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Categories_IDCategory",
                        column: x => x.IDCategory,
                        principalTable: "Categories",
                        principalColumn: "IDCategory",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillDetails",
                columns: table => new
                {
                    IDBillDetail = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDBill = table.Column<int>(type: "int", nullable: false),
                    IDProduct = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillDetails", x => x.IDBillDetail);
                    table.ForeignKey(
                        name: "FK_BillDetails_Bills_IDBill",
                        column: x => x.IDBill,
                        principalTable: "Bills",
                        principalColumn: "IDBill",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillDetails_Products_IDProduct",
                        column: x => x.IDProduct,
                        principalTable: "Products",
                        principalColumn: "IDProduct",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    IDCart = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(20)", nullable: true),
                    IDProduct = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.IDCart);
                    table.ForeignKey(
                        name: "FK_Carts_Accounts_UserName",
                        column: x => x.UserName,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Carts_Products_IDProduct",
                        column: x => x.IDProduct,
                        principalTable: "Products",
                        principalColumn: "IDProduct",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    IDFavorite = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(20)", nullable: true),
                    IDProduct = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.IDFavorite);
                    table.ForeignKey(
                        name: "FK_Favorites_Accounts_UserName",
                        column: x => x.UserName,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Favorites_Products_IDProduct",
                        column: x => x.IDProduct,
                        principalTable: "Products",
                        principalColumn: "IDProduct",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageProducts",
                columns: table => new
                {
                    IDImage = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "varchar(150)", nullable: true),
                    IDProduct = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageProducts", x => x.IDImage);
                    table.ForeignKey(
                        name: "FK_ImageProducts_Products_IDProduct",
                        column: x => x.IDProduct,
                        principalTable: "Products",
                        principalColumn: "IDProduct",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    IDRating = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(20)", nullable: true),
                    IDBillDetail = table.Column<int>(type: "int", nullable: false),
                    Star = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "ntext", nullable: true),
                    Image = table.Column<string>(type: "varchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.IDRating);
                    table.ForeignKey(
                        name: "FK_Ratings_Accounts_UserName",
                        column: x => x.UserName,
                        principalTable: "Accounts",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ratings_BillDetails_IDBillDetail",
                        column: x => x.IDBillDetail,
                        principalTable: "BillDetails",
                        principalColumn: "IDBillDetail",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AddressDetails_UserName",
                table: "AddressDetails",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_IDBill",
                table: "BillDetails",
                column: "IDBill");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_IDProduct",
                table: "BillDetails",
                column: "IDProduct");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_UerName",
                table: "Bills",
                column: "UerName");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_IDProduct",
                table: "Carts",
                column: "IDProduct");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserName",
                table: "Carts",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_IDProduct",
                table: "Favorites",
                column: "IDProduct");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserName",
                table: "Favorites",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_ImageProducts_IDProduct",
                table: "ImageProducts",
                column: "IDProduct");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserReceive",
                table: "Messages",
                column: "UserReceive");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserSend",
                table: "Messages",
                column: "UserSend");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IDCategory",
                table: "Products",
                column: "IDCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserName",
                table: "Products",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_IDBillDetail",
                table: "Ratings",
                column: "IDBillDetail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserName",
                table: "Ratings",
                column: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressDetails");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "ImageProducts");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "BillDetails");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
