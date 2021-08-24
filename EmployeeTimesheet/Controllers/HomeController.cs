using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EmployeeTimesheet.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;

namespace EmployeeTimesheet.Controllers
{
    public class HomeController : Controller
    {
        private static Logger _logger = LogManager.GetLogger("mainLoggerRules");
        Context db = new Context();

        public HomeController()
        {
        }

        public IActionResult Index(string employeeNameSearch, DateTime dateSearch, int timeSearch, string reasonSearch)
        {
            //searchString = searchString?.ToLower();
            var absences = db.Absences.ToList();
            var employees = db.Employees.ToList();
            var positions = db.Positions.ToList();
            ViewBag.Employees = new SelectList(employees, "Id", "Name");
            var applySearch = !string.IsNullOrEmpty(employeeNameSearch)
                              || dateSearch != DateTime.MinValue
                              || timeSearch != 0
                              || !string.IsNullOrEmpty(reasonSearch);
            if(applySearch)
            {
                Func<EmployeeAbsence, bool> expr = s => (string.IsNullOrEmpty(employeeNameSearch) ||
                                                        s.Employee.Name.ToLower()
                                                             .Contains(employeeNameSearch.ToLower()))
                                                         && (dateSearch == DateTime.MinValue || s.Date.Date == dateSearch.Date)
                                                         && (timeSearch == 0 || s.AbsenceHours == timeSearch)
                                                         && (string.IsNullOrEmpty(reasonSearch) ||
                                                         s.Reason.ToLower().Contains(reasonSearch.ToLower()));
                absences = absences.Where(expr).ToList();
                _logger.Info($"return Home/Index view with search");
            }
            
            _logger.Info("return Home/Index view");
            ViewBag.EmployeeNameSearch = employeeNameSearch;
            ViewBag.DateSearch = dateSearch == DateTime.MinValue ? null : dateSearch.ToString("yyyy-MM-dd");
            ViewBag.TimeSearch = timeSearch;
            ViewBag.ReasonSearch = reasonSearch;
            return View(absences);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            _logger.Info("context disposed");
            base.Dispose(disposing);
        }
        [HttpGet]
        public string GetPositionName(int id)
        {
            return db.Positions.First(s => s.Id == id).Name;
        }
        [HttpPost]
        public string CheckObjectValid(int id, DateTime date, int time, string reason)
        {
            if (id == 0 || date == DateTime.MinValue || string.IsNullOrEmpty(reason))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "Заполните обязательные поля";
            }

            if (time == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "Неверный формат поля \"Время\"";
            }
            var employeeName = db.Employees.First(s => s.Id == id).Name;
            var newAbs = new EmployeeAbsence(){AbsenceHours = time, Date = date, EmployeeId = id, Reason = reason};
            db.Absences.Add(newAbs);
            db.SaveChanges();
            _logger.Info($"Absence record for employee {employeeName} for {date.ToShortDateString()} added in db");
            return null;
        }
    }
}
