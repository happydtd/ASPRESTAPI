﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Routine.Api.Data;

namespace Routine.Api.Migrations
{
    [DbContext(typeof(RoutingDbContext))]
    [Migration("20200429011957_Initialize")]
    partial class Initialize
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("Routine.Api.Entities.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("BankruptTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Industry")
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<string>("Introduction")
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.Property<string>("Product")
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Companies");

                    b.HasData(
                        new
                        {
                            Id = new Guid("9b271f3c-4798-423d-aca5-990efad0c27a"),
                            Country = "USA",
                            Industry = "Software",
                            Introduction = "Great Company",
                            Name = "Microsoft",
                            Product = "Software"
                        },
                        new
                        {
                            Id = new Guid("13f09e4a-7c60-4364-b291-9d67bc9d2f54"),
                            Country = "USA",
                            Industry = "Internet",
                            Introduction = "Don't be evil",
                            Name = "Google",
                            Product = "Software"
                        },
                        new
                        {
                            Id = new Guid("c84add17-1cf5-436c-995b-a4b21505aca1"),
                            Country = "China",
                            Industry = "Internet",
                            Introduction = "Fubao Company",
                            Name = "Alipapa",
                            Product = "Software"
                        });
                });

            modelBuilder.Entity("Routine.Api.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmployeeNo")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(10);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<int>("Gender")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = new Guid("833c57c1-81d2-45f3-a88a-63acf996cbcc"),
                            CompanyId = new Guid("9b271f3c-4798-423d-aca5-990efad0c27a"),
                            DateOfBirth = new DateTime(1982, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G003",
                            FirstName = "Mary",
                            Gender = 1,
                            LastName = "King"
                        },
                        new
                        {
                            Id = new Guid("1585d3f4-e44d-40a7-a82b-40f540b06438"),
                            CompanyId = new Guid("9b271f3c-4798-423d-aca5-990efad0c27a"),
                            DateOfBirth = new DateTime(1977, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G004",
                            FirstName = "Daniel",
                            Gender = 1,
                            LastName = "Brawn"
                        },
                        new
                        {
                            Id = new Guid("c795686c-fec3-47aa-bfc5-1a117102e695"),
                            CompanyId = new Guid("13f09e4a-7c60-4364-b291-9d67bc9d2f54"),
                            DateOfBirth = new DateTime(1982, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G005",
                            FirstName = "Lady",
                            Gender = 1,
                            LastName = "GaGa"
                        },
                        new
                        {
                            Id = new Guid("bfccfddf-8806-4b2c-86ee-f6e3e27da865"),
                            CompanyId = new Guid("13f09e4a-7c60-4364-b291-9d67bc9d2f54"),
                            DateOfBirth = new DateTime(1977, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G006",
                            FirstName = "Michale",
                            Gender = 1,
                            LastName = "Jeke"
                        },
                        new
                        {
                            Id = new Guid("b2b0e59c-a07d-4126-b91d-14cf11fd1fe4"),
                            CompanyId = new Guid("c84add17-1cf5-436c-995b-a4b21505aca1"),
                            DateOfBirth = new DateTime(1982, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G007",
                            FirstName = "Daniel",
                            Gender = 1,
                            LastName = "Lin"
                        },
                        new
                        {
                            Id = new Guid("ed7ad799-c641-4777-89ac-be61b47a7040"),
                            CompanyId = new Guid("c84add17-1cf5-436c-995b-a4b21505aca1"),
                            DateOfBirth = new DateTime(1977, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmployeeNo = "G008",
                            FirstName = "Mark",
                            Gender = 0,
                            LastName = "Delen"
                        });
                });

            modelBuilder.Entity("Routine.Api.Entities.Employee", b =>
                {
                    b.HasOne("Routine.Api.Entities.Company", "Company")
                        .WithMany("Employees")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}