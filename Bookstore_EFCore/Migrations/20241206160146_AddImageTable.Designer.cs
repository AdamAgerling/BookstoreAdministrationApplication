﻿// <auto-generated />
using System;
using BookstoreAdmin.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookstoreAdmin.Migrations
{
    [DbContext(typeof(BookstoreDbContext))]
    [Migration("20241206160146_AddImageTable")]
    partial class AddImageTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookstoreAdmin.Model.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AuthorId"));

                    b.Property<string>("AuthorBirthCountry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("AuthorBirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("AuthorDeathDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("AuthorLastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthorName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Book", b =>
                {
                    b.Property<string>("ISBN13")
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int?>("BookLanguageLanguageId")
                        .HasColumnType("int");

                    b.Property<decimal>("BookPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("BookRelease")
                        .HasColumnType("datetime2");

                    b.Property<string>("BookTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<int>("PublisherId")
                        .HasColumnType("int");

                    b.Property<int?>("PublisherId1")
                        .HasColumnType("int");

                    b.HasKey("ISBN13");

                    b.HasIndex("AuthorId");

                    b.HasIndex("BookLanguageLanguageId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("PublisherId");

                    b.HasIndex("PublisherId1");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.BookLanguage", b =>
                {
                    b.Property<int>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LanguageId"));

                    b.Property<string>("LanguageName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LanguageId");

                    b.ToTable("BookLanguages");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Image", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<byte[]>("ImageArray")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.InventoryBalance", b =>
                {
                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("ISBN13")
                        .HasColumnType("nvarchar(13)");

                    b.Property<int>("BookQuantity")
                        .HasColumnType("int");

                    b.HasKey("StoreId", "ISBN13");

                    b.HasIndex("ISBN13");

                    b.ToTable("InventoryBalances");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Publisher", b =>
                {
                    b.Property<int>("PublisherId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PublisherId"));

                    b.Property<string>("PublisherCountry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PublisherName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PublisherId");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.PurchaseHistory", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"));

                    b.Property<string>("ISBN13")
                        .IsRequired()
                        .HasColumnType("nvarchar(13)");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PurchaseQuantity")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("PurchaseId");

                    b.HasIndex("ISBN13");

                    b.HasIndex("StoreId");

                    b.ToTable("PurchaseHistories");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Store", b =>
                {
                    b.Property<int>("StoreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreId"));

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StoreStreetAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StoreId");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Book", b =>
                {
                    b.HasOne("BookstoreAdmin.Model.Author", "Author")
                        .WithMany("Books")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookstoreAdmin.Model.BookLanguage", null)
                        .WithMany("Books")
                        .HasForeignKey("BookLanguageLanguageId");

                    b.HasOne("BookstoreAdmin.Model.BookLanguage", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookstoreAdmin.Model.Publisher", "Publisher")
                        .WithMany()
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookstoreAdmin.Model.Publisher", null)
                        .WithMany("Books")
                        .HasForeignKey("PublisherId1");

                    b.Navigation("Author");

                    b.Navigation("Language");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.InventoryBalance", b =>
                {
                    b.HasOne("BookstoreAdmin.Model.Book", "Book")
                        .WithMany()
                        .HasForeignKey("ISBN13")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookstoreAdmin.Model.Store", "Store")
                        .WithMany("InventoryBalances")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.PurchaseHistory", b =>
                {
                    b.HasOne("BookstoreAdmin.Model.Book", "Book")
                        .WithMany()
                        .HasForeignKey("ISBN13")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookstoreAdmin.Model.Store", "Store")
                        .WithMany("PurchaseHistories")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Author", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.BookLanguage", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Publisher", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("BookstoreAdmin.Model.Store", b =>
                {
                    b.Navigation("InventoryBalances");

                    b.Navigation("PurchaseHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
