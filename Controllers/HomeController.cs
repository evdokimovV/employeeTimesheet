using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EmployeeTimesheet.Basics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EmployeeTimesheet.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;

namespace EmployeeTimesheet.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// записывает логи в bin\Debug\netcoreapp3.1\logs
        /// </summary>
        private static Logger _logger = LogManager.GetLogger("mainLoggerRules");
        Context db = new Context();

        public HomeController()
        {
        }

        /// <summary>
        /// Возвращает index с примененным поиском по значениям
        /// </summary>
        /// <param name="employeeNameSearch">строка поиска по имени сотрудника</param>
        /// <param name="dateSearch">дата для поиска</param>
        /// <param name="timeSearch">время отсутствия</param>
        /// <param name="reasonSearch">причина</param>
        /// <returns></returns>
        public IActionResult Index(string employeeNameSearch, DateTime dateSearch, int timeSearch, string reasonSearch)
        {
            var absences = db.Absences.ToList();
            var employees = db.Employees.ToList();
            var positions = db.Positions.ToList();
            ViewBag.Employees = new SelectList(employees, nameof(Employee.Id), nameof(Employee.Name));
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
                _logger.Info($"return Home/Index View with search");
            }
            
            _logger.Info("return Home/Index View");
            ViewBag.EmployeeNameSearch = employeeNameSearch;
            ViewBag.DateSearch = dateSearch == DateTime.MinValue ? null : dateSearch.ToString(TimesheetTableConstants.DefaultDateFormat);
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
            _logger.Info("Context disposed");
            base.Dispose(disposing);
        }
        /// <summary>
        /// получение имени должности по коду
        /// </summary>
        /// <param name="id">код должности</param>
        /// <returns></returns>
        [HttpGet]
        public string GetPositionName(int id)
        {
            try
            {
                return db.Positions.First(s => s.Id == id).Name;
            }
            catch(Exception e)
            {
                _logger.Info($"Error on find employee position by Id: {e.Message}");
            }

            return null;
        }
        /// <summary>
        /// метод сохранения с проверкой на валидность
        /// </summary>
        /// <param name="employeeId">код сотрудника</param>
        /// <param name="date">дата</param>
        /// <param name="time">время отсутствия</param>
        /// <param name="reason">причина</param>
        /// <returns></returns>
        [HttpPost]
        public string SaveAbsence(int employeeId, DateTime date, int time, string reason)
        {
            if (employeeId == 0 || date == DateTime.MinValue || string.IsNullOrEmpty(reason))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return TimesheetTableConstants.FillRequiredFields;
            }

            if (time == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return TimesheetTableConstants.IncorectTimeFieldFormat;
            }

            try
            {
                var employeeName = db.Employees.First(s => s.Id == employeeId).Name;
                var newAbs = new EmployeeAbsence() { AbsenceHours = time, Date = date, EmployeeId = employeeId, Reason = reason };
                db.Absences.Add(newAbs);
                db.SaveChanges();
                _logger.Info($"Absence record for employee {employeeName} for {date.ToShortDateString()} added in db");
            }
            catch (Exception e)
            {
                _logger.Info($"Error on save in database: {e.Message}");
            }
            return null;
        }
    }
}
