using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                    new Employee
                    {
                        Id = 2,
                        Name = "Bithal",
                        Department = DepartmentType.IT,
                        Email = "bithal@gmail.com"
                    },
                    new Employee
                    {
                        Id = 3,
                        Name = "Janardhan",
                        Department = DepartmentType.HR,
                        Email = "janardhan@gmail.com"
                    }
                );
        }
    }
}
