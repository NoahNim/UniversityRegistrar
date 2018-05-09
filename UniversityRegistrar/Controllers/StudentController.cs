using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Controllers
{
    public class StudentController : Controller
    {

        [HttpGet("/students/index")]
        public ActionResult Index()
        {
            List<Student> allStudents = Student.GetAll();
            return View(allStudents);
        }
        [HttpGet("/students/new")]
        public ActionResult CreateForm()
        {
            return View();
        }
        [HttpPost("/students")]
        public ActionResult Create()
        {
            Student newStudent = new Student(Request.Form["student-name"], Request.Form["student-date"]);
            newStudent.Save();
            return RedirectToAction("Success", "Home");
        }
    }
}
