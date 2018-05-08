using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Models
{
  public class Student
  {
    private int _student_id;
    private string _name;
    private string _date;

    public Student(string name, string date, int student_id = 0)
    {
      _student_id = student_id;
      _name = name;
      _date = date;
    }

    public string GetName()
    {
        return _name;
    }

    public int GetId()
    {
        return _student_id;
    }
    public string GetDate()
    {
        return _date;
    }
    public void Save()
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"INSERT INTO students (name, date) VALUES (@name, @date);";

        MySqlParameter name = new MySqlParameter();
        name.ParameterName = "@name";
        name.Value = this._name;
        cmd.Parameters.Add(name);

        MySqlParameter date= new MySqlParameter();
        date.ParameterName = "@date";
        date.Value = this._date;
        cmd.Parameters.Add(date);

        cmd.ExecuteNonQuery();
        _student_id = (int) cmd.LastInsertedId;
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
    }
    public static List<Student> GetAll()
    {
        List<Student> allStudents = new List<Student> {};
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT * FROM students;";
        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        while(rdr.Read())
        {
          int studentId = rdr.GetInt32(0);
          string studentName = rdr.GetString(1);
          string studentDate = rdr.GetString(2);

          Student newStudent = new Student(studentName, studentDate, studentId);
          allStudents.Add(newStudent);
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return allStudents;
    }
    public void AddCourse(Course newCourse)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"INSERT INTO course_info (student_id, course_id) VALUES (@studentId, @CourseId);";

        MySqlParameter student_id = new MySqlParameter();
        student_id.ParameterName = "@StudentId";
        student_id.Value = _student_id;
        cmd.Parameters.Add(student_id);

        MySqlParameter course_id = new MySqlParameter();
        course_id.ParameterName = "@CourseId";
        course_id.Value = newCourse.GetId();
        cmd.Parameters.Add(course_id);

        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
    }
    public List<Course> GetCourses()
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT courses.* FROM students
        JOIN course_info ON (students.student_id = course_info.student_id)
        JOIN courses ON(course_info.course_id = courses.course_id)
        WHERE students.student_id = @StudentId";

        MySqlParameter studentIdParameter = new MySqlParameter();
        studentIdParameter.ParameterName = "@studentId";
        studentIdParameter.Value = _student_id;
        cmd.Parameters.Add(studentIdParameter);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;

        List<int> courseIds = new List<int> {};
        while(rdr.Read())
        {
            int courseId = rdr.GetInt32(0);
            courseIds.Add(courseId);
        }
        rdr.Dispose();

        List<Course> courses = new List<Course> {};
        foreach (int courseId in courseIds)
        {
            var courseQuery = conn.CreateCommand() as MySqlCommand;
            courseQuery.CommandText = @"SELECT * FROM courses WHERE course_id = @CourseId;";

            MySqlParameter courseIdParameter = new MySqlParameter();
            courseIdParameter.ParameterName = "@CourseId";
            courseIdParameter.Value = courseId;
            courseQuery.Parameters.Add(courseIdParameter);

            var courseQueryRdr = courseQuery.ExecuteReader() as MySqlDataReader;
            while(courseQueryRdr.Read())
            {
                int thisCourseId = courseQueryRdr.GetInt32(0);
                string courseName = courseQueryRdr.GetString(1);
                string courseNumber = courseQueryRdr.GetString(2);
                Course foundCourse = new Course(courseName, courseNumber, thisCourseId);
                courses.Add(foundCourse);
            }
            courseQueryRdr.Dispose();
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return courses;
    }
    public static Student Find(int student_id)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT * FROM students WHERE student_id = (@searchId);";

        MySqlParameter searchId = new MySqlParameter();
        searchId.ParameterName = "@searchId";
        searchId.Value = student_id;
        cmd.Parameters.Add(searchId);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        int studentId = 0;
        string studentName = "";
        string studentDate = "";

        while(rdr.Read())
        {
          studentId = rdr.GetInt32(0);
          studentName = rdr.GetString(1);
          studentDate= rdr.GetString(2);
        }

        Student newStudent = new Student(studentName, studentDate, studentId);
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }

        return newStudent;
    }
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM students WHERE student_id = @StudentId; DELETE FROM course_info WHERE student_id = @StudentId;";

      MySqlParameter studentIdParameter = new MySqlParameter();
      studentIdParameter.ParameterName = "@StudentId";
      studentIdParameter.Value = this.GetId();
      cmd.Parameters.Add(studentIdParameter);

      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }
    public static void DeleteAll()
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"DELETE FROM students;";
        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
    }
    public override bool Equals(System.Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
        return false;
      }
      else
      {
         Student newStudent = (Student) otherStudent;
         bool idEquality = this.GetId() == newStudent.GetId();
         bool nameEquality = this.GetName() == newStudent.GetName();
         return (idEquality && nameEquality);
       }
    }
    public override int GetHashCode()
    {
         return this.GetName().GetHashCode();
    }

  }
}
