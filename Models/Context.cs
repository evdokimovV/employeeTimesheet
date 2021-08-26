using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTimesheet.Models
{
    public class Context : DbContext
    {
        internal DbSet<Employee> Employees { get; set; }
        internal DbSet<EmployeePosition> Positions { get; set; }
        internal DbSet<EmployeeAbsence> Absences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "mydb.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            options.UseSqlite(connection);
        }

    }
}
