using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using UniversityRegistrar.Models;
using UniversityRegistrar;
using MySql.Data.MySqlClient;

namespace UniversityRegistrar.Tests
{
  [TestClass]
  public class UniversityTest : IDisposable
  {
    public UniversityTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=university_test;";
    }
    public void Dispose()
    {
      Student.DeleteAll();
      Course.DeleteAll();
    }
    [TestMethod]
    public void Save_StudentSavesToDatabase_Students()
    {
      //Arrange
      Student testStudent = new Student("Tim", "08/11/21");
      testStudent.Save();

      //Act
      List<Student> studentResult = Student.GetAll();
      List<Student> studentList = new List<Student>{testStudent};

      //Assert
      CollectionAssert.AreEqual(studentList, studentResult);
    }
    [TestMethod]
    public void Find_FindsStudentInDatabase_Students()
    {
      //Arrange
      Student testStudent = new Student("Kyla", "08/04/19");
      testStudent.Save();

      //Act
      Student result = Student.Find(testStudent.GetId());

      //Assert
      Assert.AreEqual(testStudent, result);
    }
    [TestMethod]
    public void Delete_DeleletesStudentInDataBase_Students()
    {
      Student testStudent = new Student("Jack", "08/11/19");
      testStudent.Save();
      Student otherStudent = new Student("Kyle", "07/12/18");
      otherStudent.Save();

      testStudent.Delete();
      List<Student> result = Student.GetAll();
      List<Student> expected = new List<Student>{otherStudent};

      CollectionAssert.AreEqual(expected, result);
    }
    [TestMethod]
    public void Save_CourseSavesToDatabase_Courses()
    {
      //Arrange
      Course testCourse = new Course("History Studies", "101");
      testCourse.Save();

      //Act
      List<Course> courseResult = Course.GetAll();
      List<Course> courseList = new List<Course>{testCourse};

      //Assert
      CollectionAssert.AreEqual(courseList, courseResult);
    }
    [TestMethod]
    public void Find_FindsCourseInDatabase_Courses()
    {
      //Arrange
      Course testCourse = new Course("Math", "105");
      testCourse.Save();

      //Act
      Course result = Course.Find(testCourse.GetId());

      //Assert
      Assert.AreEqual(testCourse, result);
    }


  }
}
