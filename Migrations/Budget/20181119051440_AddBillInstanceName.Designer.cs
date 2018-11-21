﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleBillPay;

namespace SimpleBillPay.Migrations.Budget
{
    [DbContext(typeof(BudgetContext))]
    [Migration("20181119051440_AddBillInstanceName")]
    partial class AddBillInstanceName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SimpleBillPay.Areas.Identity.Data.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("AccountCreationDateTime");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("SimpleBillPay.Models.BillInstance", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<int>("BillTemplateID");

                    b.Property<DateTime>("DueDate");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("BillTemplateID");

                    b.ToTable("BillInstance");
                });

            modelBuilder.Entity("SimpleBillPay.Models.BillTemplate", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<int>("FrequencyInMonths");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("BillTemplate");
                });

            modelBuilder.Entity("SimpleBillPay.Models.Payment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount");

                    b.Property<int>("BillInstanceID");

                    b.Property<DateTime>("PaymentDate");

                    b.HasKey("ID");

                    b.HasIndex("BillInstanceID");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("SimpleBillPay.Models.BillInstance", b =>
                {
                    b.HasOne("SimpleBillPay.Models.BillTemplate", "BillTemplate")
                        .WithMany()
                        .HasForeignKey("BillTemplateID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SimpleBillPay.Models.BillTemplate", b =>
                {
                    b.HasOne("SimpleBillPay.Areas.Identity.Data.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SimpleBillPay.Models.Payment", b =>
                {
                    b.HasOne("SimpleBillPay.Models.BillInstance", "BillInstance")
                        .WithMany("Payments")
                        .HasForeignKey("BillInstanceID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
