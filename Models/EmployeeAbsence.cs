using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTimesheet.Models
{
    internal class EmployeeAbsence
    {
        internal int Id { get; set; }
        internal int EmployeeId { get; set; }
        internal DateTime Date { get; set; }
        internal int AbsenceHours { get; set; }
        internal string Reason { get; set; }

        internal virtual Employee Employee { get; set; }


    }
}
