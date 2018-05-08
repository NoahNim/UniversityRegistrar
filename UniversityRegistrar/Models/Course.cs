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
    public int GetCourseId()
    {
      return _course_id;
    }
    public string GetCourseName()
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
      cmd.CommandText = @"INSERT INTO courses (courseName, courseNum) VALUES (@name, @coursenum);";

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
          Course newCourse = new Course(courseName, courseNum, courseIdf);
          allCourse.Add(newCourse);
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return allcourse;
    }

  }
}
