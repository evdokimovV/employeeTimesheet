using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTimesheet.Models
{
    public class EmployeeAbsence
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public int AbsenceHours { get; set; }
        public string Reason { get; set; }

        public virtual Employee Employee { get; set; }


    }
}
