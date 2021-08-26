using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTimesheet.Models
{
    internal class Employee
    {
        internal Employee()
        {
            Absences = new List<EmployeeAbsence>();
        }
        internal int Id { get; set; }
        internal string Name { get; set; }

        internal int PositionId { get; set; }
        internal EmployeePosition Position { get; set; }
        internal ICollection<EmployeeAbsence> Absences { get; set; }
    }
}
