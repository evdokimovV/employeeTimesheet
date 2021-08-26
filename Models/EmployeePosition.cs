using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTimesheet.Models
{
    internal class EmployeePosition
    {
        internal EmployeePosition()
        {
            Employees = new List<Employee>();
        }
        internal int Id { get; set; }
        internal string Name { get; set; }
        internal ICollection<Employee> Employees { get; set; }
    }
}
