﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SearchNavigate.Infrastructure.Persistence.Context;

#nullable disable

namespace SearchNavigate.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(SearchNavigateDbContext))]
    [Migration("20240803191559_FirstMigraiton")]
    partial class FirstMigraiton
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Category", "DBO");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("OrderItemsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("OrderedBy")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OrderedBy");

                    b.ToTable("Order", "DBO");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.OrderItems", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems", "DBO");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product", "DBO");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("User", "DBO");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.Order", b =>
                {
                    b.HasOne("SearchNavigate.Core.Domain.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("OrderedBy")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.OrderItems", b =>
                {
                    b.HasOne("SearchNavigate.Core.Domain.Models.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SearchNavigate.Core.Domain.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.Product", b =>
                {
                    b.HasOne("SearchNavigate.Core.Domain.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("SearchNavigate.Core.Domain.Models.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
