using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTimesheet.Models
{
    public class Employee
    {
        public Employee()
        {
            Absences = new List<EmployeeAbsence>();
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public int PositionId { get; set; }
        public EmployeePosition Position { get; set; }
        public ICollection<EmployeeAbsence> Absences { get; set; }
    }
}
