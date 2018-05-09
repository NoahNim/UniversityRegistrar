using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Controllers
{
    public class CourseController : Controller
    {

        [HttpGet("/courses/index")]
        public ActionResult Index()
        {
            List<Course> allCourses = Course.GetAll();
            return View(allCourses);
        }
        [HttpGet("/courses/new")]
        public ActionResult CreateForm()
        {
            return View();
        }
        [HttpPost("/courses")]
        public ActionResult Create()
        {
            Course newCourse = new Course(Request.Form["course-name"], Request.Form["course-num"]);
            newCourse.Save();
            return RedirectToAction("Success", "Home");
        }
    }
}
