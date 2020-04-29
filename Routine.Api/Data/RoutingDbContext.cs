using Microsoft.EntityFrameworkCore;
using Routine.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Routine.Api.Data
{
    public class RoutingDbContext : DbContext
    {
        public RoutingDbContext(DbContextOptions<RoutingDbContext> options) : base(options)
        {

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(x => x.Country).HasMaxLength(50);
            modelBuilder.Entity<Company>().Property(x => x.Industry).HasMaxLength(50);
            modelBuilder.Entity<Company>().Property(x => x.Product).HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(x => x.Introduction).HasMaxLength(500);
            modelBuilder.Entity<Employee>().Property(x => x.EmployeeNo).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Employee>().Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(x => x.LastName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>().HasOne(navigationExpression: x => x.Company)
                .WithMany(navigationExpression: x => x.Employees)
                //company删了后， employee会自动一起删除
                .HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = Guid.Parse("9b271f3c-4798-423d-aca5-990efad0c27a"),
                    Name = "Microsoft",
                    Introduction = "Great Company",
                    Country = "USA",
                    Industry = "Software",
                    Product = "Software"
                },
                new Company
                {
                    Id = Guid.Parse("13f09e4a-7c60-4364-b291-9d67bc9d2f54"),
                    Name = "Google",
                    Introduction = "Don't be evil",
                    Country = "USA",
                    Industry = "Internet",
                    Product = "Software"
                },
                new Company
                {
                    Id = Guid.Parse("c84add17-1cf5-436c-995b-a4b21505aca1"),
                    Name = "Alipapa",
                    Introduction = "Fubao Company",
                    Country = "China",
                    Industry = "Internet",
                    Product = "Software"
                }
                );
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = Guid.Parse("833c57c1-81d2-45f3-a88a-63acf996cbcc"),
                    CompanyId = Guid.Parse("9b271f3c-4798-423d-aca5-990efad0c27a"),
                    DateOfBirth = new DateTime(year: 1982, month: 11, day: 4),
                    EmployeeNo = "G003",
                    FirstName = "Mary",
                    LastName = "King",
                    Gender = Gender.Female
                },
                        new Employee
                        {
                            Id = Guid.Parse("1585d3f4-e44d-40a7-a82b-40f540b06438"),
                            CompanyId = Guid.Parse("9b271f3c-4798-423d-aca5-990efad0c27a"),
                            DateOfBirth = new DateTime(year: 1977, month: 3, day: 2),
                            EmployeeNo = "G004",
                            FirstName = "Daniel",
                            LastName = "Brawn",
                            Gender = Gender.Female
                        },
                    new Employee
                    {
                        Id = Guid.Parse("c795686c-fec3-47aa-bfc5-1a117102e695"),
                        CompanyId = Guid.Parse("13f09e4a-7c60-4364-b291-9d67bc9d2f54"),
                        DateOfBirth = new DateTime(year: 1982, month: 11, day: 4),
                        EmployeeNo = "G005",
                        FirstName = "Lady",
                        LastName = "GaGa",
                        Gender = Gender.Female
                    },
                        new Employee
                        {
                            Id = Guid.Parse("bfccfddf-8806-4b2c-86ee-f6e3e27da865"),
                            CompanyId = Guid.Parse("13f09e4a-7c60-4364-b291-9d67bc9d2f54"),
                            DateOfBirth = new DateTime(year: 1977, month: 3, day: 2),
                            EmployeeNo = "G006",
                            FirstName = "Michale",
                            LastName = "Jeke",
                            Gender = Gender.Female
                        },

                        new Employee
                        {
                            Id = Guid.Parse("b2b0e59c-a07d-4126-b91d-14cf11fd1fe4"),
                            CompanyId = Guid.Parse("c84add17-1cf5-436c-995b-a4b21505aca1"),
                            DateOfBirth = new DateTime(year: 1982, month: 11, day: 4),
                            EmployeeNo = "G007",
                            FirstName = "Daniel",
                            LastName = "Lin",
                            Gender = Gender.Female
                        },
                        new Employee
                        {
                            Id = Guid.Parse("ed7ad799-c641-4777-89ac-be61b47a7040"),
                            CompanyId = Guid.Parse("c84add17-1cf5-436c-995b-a4b21505aca1"),
                            DateOfBirth = new DateTime(year: 1977, month: 3, day: 2),
                            EmployeeNo = "G008",
                            FirstName = "Mark",
                            LastName = "Delen",
                            Gender = Gender.Male
                        }



                );
        }
    }
}
