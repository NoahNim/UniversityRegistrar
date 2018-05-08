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

  }
}
