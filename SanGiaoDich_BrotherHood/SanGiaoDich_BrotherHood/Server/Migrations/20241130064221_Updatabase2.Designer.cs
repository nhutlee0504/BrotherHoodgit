﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SanGiaoDich_BrotherHood.Server.Data;

namespace SanGiaoDich_BrotherHood.Server.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241130064221_Updatabase2")]
    partial class Updatabase2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Account", b =>
                {
                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Dob")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Doe")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(6)");

                    b.Property<string>("Home")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Introduce")
                        .HasColumnType("ntext");

                    b.Property<bool?>("IsActived")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Nationality")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(12)");

                    b.Property<decimal?>("PreSystem")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("TimeBanned")
                        .HasColumnType("datetime2");

                    b.HasKey("UserName");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.AddressDetail", b =>
                {
                    b.Property<int>("IDAddress")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdditionalDetail")
                        .IsRequired()
                        .HasColumnType("Nvarchar(50)");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("Nvarchar(50)");

                    b.Property<string>("ProvinceCity")
                        .IsRequired()
                        .HasColumnType("Nvarchar(50)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Wardcommune")
                        .IsRequired()
                        .HasColumnType("Nvarchar(50)");

                    b.HasKey("IDAddress");

                    b.HasIndex("UserName");

                    b.ToTable("AddressDetails");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Bill", b =>
                {
                    b.Property<int>("IDBill")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateReceipt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeliveryAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(150)");

                    b.Property<string>("FullName")
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentType")
                        .HasColumnType("nvarchar(70)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(12)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("IDBill");

                    b.HasIndex("UserName");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.BillDetail", b =>
                {
                    b.Property<int>("IDBillDetail")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IDBill")
                        .HasColumnType("int");

                    b.Property<int>("IDProduct")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("IDBillDetail");

                    b.HasIndex("IDBill");

                    b.HasIndex("IDProduct");

                    b.ToTable("BillDetails");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Cart", b =>
                {
                    b.Property<int>("IDCart")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("IDCart");

                    b.HasIndex("UserName");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.CartItem", b =>
                {
                    b.Property<int>("CartItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IDCart")
                        .HasColumnType("int");

                    b.Property<int>("IDProduct")
                        .HasColumnType("int");

                    b.HasKey("CartItemID");

                    b.HasIndex("IDCart");

                    b.HasIndex("IDProduct");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Category", b =>
                {
                    b.Property<int>("IDCategory")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NameCate")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserUpdated")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IDCategory");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Conversation", b =>
                {
                    b.Property<int>("ConversationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("UserGive")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ConversationID");

                    b.HasIndex("Username");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.ConversationParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ConversationId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoinedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ConversationParticipants");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Favorite", b =>
                {
                    b.Property<int>("IDFavorite")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IDProduct")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("IDFavorite");

                    b.HasIndex("IDProduct");

                    b.HasIndex("UserName");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.ImageProduct", b =>
                {
                    b.Property<int>("IDImage")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("IDProduct")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("bit");

                    b.HasKey("IDImage");

                    b.HasIndex("IDProduct");

                    b.ToTable("ImageProducts");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Messages", b =>
                {
                    b.Property<int>("MessageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ConversationID")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserSend")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MessageID");

                    b.HasIndex("ConversationID");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.PaymentRequestModel", b =>
                {
                    b.Property<int>("PaymentReqID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("OrderDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TxnRef")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PaymentReqID");

                    b.HasIndex("UserName");

                    b.ToTable("PaymentRequests");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.PaymentResponseModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsProcessed")
                        .HasColumnType("bit");

                    b.Property<string>("OrderDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PaymentReqID")
                        .HasColumnType("int");

                    b.Property<bool>("Success")
                        .HasColumnType("bit");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VnPayResponseCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PaymentReqID")
                        .IsUnique();

                    b.ToTable("PaymentResponses");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Product", b =>
                {
                    b.Property<int>("IDProduct")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountAccept")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date");

                    b.Property<int>("IDCategory")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PriceUp")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProrityLevel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("IDProduct");

                    b.HasIndex("IDCategory");

                    b.HasIndex("UserName");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Rating", b =>
                {
                    b.Property<int>("IDRating")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment")
                        .HasColumnType("ntext");

                    b.Property<int>("IDBillDetail")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("varchar(150)");

                    b.Property<int>("Star")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("IDRating");

                    b.HasIndex("IDBillDetail")
                        .IsUnique();

                    b.HasIndex("UserName");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Report", b =>
                {
                    b.Property<int>("ReportID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReportID");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.AddressDetail", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("addressDetails")
                        .HasForeignKey("UserName");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Bill", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("bills")
                        .HasForeignKey("UserName");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.BillDetail", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Bill", "Bill")
                        .WithMany("billDetails")
                        .HasForeignKey("IDBill")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Product", "Product")
                        .WithMany("billDetails")
                        .HasForeignKey("IDProduct")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bill");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Cart", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("carts")
                        .HasForeignKey("UserName");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.CartItem", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Cart", "Cart")
                        .WithMany("cartitem")
                        .HasForeignKey("IDCart")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Product", "Product")
                        .WithMany("cartItem")
                        .HasForeignKey("IDProduct")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Conversation", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("conversations")
                        .HasForeignKey("Username");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Favorite", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Product", "Product")
                        .WithMany("favorites")
                        .HasForeignKey("IDProduct")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("favorites")
                        .HasForeignKey("UserName");

                    b.Navigation("Account");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.ImageProduct", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Product", "Product")
                        .WithMany("imageProducts")
                        .HasForeignKey("IDProduct")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Messages", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Conversation", "conversation")
                        .WithMany("message")
                        .HasForeignKey("ConversationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("conversation");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.PaymentRequestModel", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("paymentRequests")
                        .HasForeignKey("UserName");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.PaymentResponseModel", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.PaymentRequestModel", "PaymentRequest")
                        .WithOne("PaymentResponse")
                        .HasForeignKey("SanGiaoDich_BrotherHood.Shared.Models.PaymentResponseModel", "PaymentReqID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PaymentRequest");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Product", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Category", "Category")
                        .WithMany("products")
                        .HasForeignKey("IDCategory")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("products")
                        .HasForeignKey("UserName");

                    b.Navigation("Account");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Rating", b =>
                {
                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.BillDetail", "BillDetail")
                        .WithOne("Rating")
                        .HasForeignKey("SanGiaoDich_BrotherHood.Shared.Models.Rating", "IDBillDetail")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SanGiaoDich_BrotherHood.Shared.Models.Account", "Account")
                        .WithMany("ratings")
                        .HasForeignKey("UserName");

                    b.Navigation("Account");

                    b.Navigation("BillDetail");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Account", b =>
                {
                    b.Navigation("addressDetails");

                    b.Navigation("bills");

                    b.Navigation("carts");

                    b.Navigation("conversations");

                    b.Navigation("favorites");

                    b.Navigation("paymentRequests");

                    b.Navigation("products");

                    b.Navigation("ratings");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Bill", b =>
                {
                    b.Navigation("billDetails");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.BillDetail", b =>
                {
                    b.Navigation("Rating");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Cart", b =>
                {
                    b.Navigation("cartitem");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Category", b =>
                {
                    b.Navigation("products");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Conversation", b =>
                {
                    b.Navigation("message");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.PaymentRequestModel", b =>
                {
                    b.Navigation("PaymentResponse");
                });

            modelBuilder.Entity("SanGiaoDich_BrotherHood.Shared.Models.Product", b =>
                {
                    b.Navigation("billDetails");

                    b.Navigation("cartItem");

                    b.Navigation("favorites");

                    b.Navigation("imageProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
