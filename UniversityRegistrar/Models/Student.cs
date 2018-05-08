using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

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

        // Code to declare, set, and add values to a categoryId SQL parameters has also been removed.

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
          // We no longer need to read categoryIds from our items table here.
          // Constructor below no longer includes a itemCategoryId parameter:
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
         // We no longer compare Students' categoryIds in a categoryEquality bool here.
         return (idEquality && nameEquality);
       }
    }
    public override int GetHashCode()
    {
         return this.GetName().GetHashCode();
    }

  }
}
