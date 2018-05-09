using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Models
{
  public class Course
  {
    private int _course_id;
    private string _courseName;
    private string _courseNum;

    public Course(string courseName, string courseNum, int courseID = 0)
    {
      _course_id = courseID;
      _courseName = courseName;
      _courseNum = courseNum;
    }
    public int GetId()
    {
      return _course_id;
    }
    public string GetName()
    {
      return _courseName;
    }
    public string GetCourseNum()
    {
      return _courseNum;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO courses (name, courseNum) VALUES (@name, @coursenum);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = this._courseName;
      cmd.Parameters.Add(name);

      MySqlParameter coursenum = new MySqlParameter();
      coursenum.ParameterName = "@coursenum";
      coursenum.Value = this._courseNum;
      cmd.Parameters.Add(coursenum);

      cmd.ExecuteNonQuery();
      _course_id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }
    public static List<Course> GetAll()
    {
        List<Course> allCourses = new List<Course> {};
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT * FROM courses;";
        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        while(rdr.Read())
        {
          int courseId = rdr.GetInt32(0);
          string courseName = rdr.GetString(1);
          string courseNum = rdr.GetString(2);
          // We no longer need to read categoryIds from our items table here.
          // Constructor below no longer includes a itemCategoryId parameter:
          Course newCourse = new Course(courseName, courseNum, courseId);
          allCourses.Add(newCourse);
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return allCourses;
    }
    public void AddStudent(Student newStudent)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"INSERT INTO course_info (student_id, course_id) VALUES (@studentId, @CourseId);";

        MySqlParameter student_id = new MySqlParameter();
        student_id.ParameterName = "@StudentId";
        student_id.Value = newStudent.GetId();
        cmd.Parameters.Add(student_id);

        MySqlParameter course_id = new MySqlParameter();
        course_id.ParameterName = "@CourseId";
        course_id.Value = _course_id;
        cmd.Parameters.Add(course_id);

        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
    }
    public List<Student> GetStudents()
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT students.* FROM courses
            JOIN course_info ON (courses.course_id = course_info.course_id)
            JOIN students ON (course_info.student_id = students.student_id)
            WHERE courses.course_id = @CourseId;";

        MySqlParameter courseIdParameter = new MySqlParameter();
        courseIdParameter.ParameterName = "@CourseId";
        courseIdParameter.Value = _course_id;
        cmd.Parameters.Add(courseIdParameter);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;

        List<int> studentIds = new List<int> {};
        while(rdr.Read())
        {
            int studentId = rdr.GetInt32(0);
            studentIds.Add(studentId);
        }
        rdr.Dispose();

        List<Student> students = new List<Student> {};
        foreach (int studentId in studentIds)
        {
            var studentQuery = conn.CreateCommand() as MySqlCommand;
            studentQuery.CommandText = @"SELECT * FROM students WHERE student_id = @StudentId;";

            MySqlParameter studentIdParameter = new MySqlParameter();
            studentIdParameter.ParameterName = "@StudentId";
            studentIdParameter.Value = studentId;
            studentQuery.Parameters.Add(studentIdParameter);

            var studentQueryRdr = studentQuery.ExecuteReader() as MySqlDataReader;
            while(studentQueryRdr.Read())
            {
                int thisStudentId = studentQueryRdr.GetInt32(0);
                string studentName = studentQueryRdr.GetString(1);
                string studentDate = studentQueryRdr.GetString(2);
                Student foundStudent = new Student(studentName, studentDate, thisStudentId);
                students.Add(foundStudent);
            }
            studentQueryRdr.Dispose();
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return students;
    }
    public static Course Find(int course_id)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT * FROM courses WHERE course_id = (@searchId);";

        MySqlParameter searchId = new MySqlParameter();
        searchId.ParameterName = "@searchId";
        searchId.Value = course_id;
        cmd.Parameters.Add(searchId);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        int courseId = 0;
        string courseName = "";
        string courseNumber = "";

        while(rdr.Read())
        {
          courseId = rdr.GetInt32(0);
          courseName = rdr.GetString(1);
          courseNumber = rdr.GetString(2);
        }

        Course newCourse = new Course(courseName, courseNumber, courseId);
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }

        return newCourse;
    }
    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM courses WHERE course_id = @courseId; DELETE FROM course_info WHERE course_id = @CourseId;";

      MySqlParameter courseIdParameter = new MySqlParameter();
      courseIdParameter.ParameterName = "@CourseId";
      courseIdParameter.Value = this.GetId();
      cmd.Parameters.Add(courseIdParameter);

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
        cmd.CommandText = @"DELETE FROM courses;";
        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
    }
    public override bool Equals(System.Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
         Course newCourse = (Course) otherCourse;
         bool idEquality = this.GetId() == newCourse.GetId();
         bool nameEquality = this.GetName() == newCourse.GetName();
         // We no longer compare Courses' categoryIds in a categoryEquality bool here.
         return (idEquality && nameEquality);
       }
    }
    public override int GetHashCode()
    {
         return this.GetName().GetHashCode();
    }

  }
}
