using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace ZlPos.Dao
{
    public class DatabaseContext : DbContext
    {
        //public DatabaseContext()
        //{

        //}

        public DbSet<Employee> Employees { get; set; }
    }

    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
