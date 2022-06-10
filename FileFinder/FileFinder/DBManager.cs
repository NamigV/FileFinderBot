using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FileFinder
{
    class DBManager
    {
        readonly SqlConnection sqlConnectionFilesManager = new SqlConnection("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

        SqlCommand command;
        SqlDataReader reader;

        public void SaveFilesIDToDB(int course_id, int file_id)
        {
            string query = "IF NOT EXISTS(SELECT * FROM FilesManager " +
                                            $"WHERE files_id = {file_id}) " +
                            "INSERT INTO FilesManager(course_id, files_id) " +
                            $"VALUES({course_id}, {file_id})";

            sqlConnectionFilesManager.Open();

            command = new SqlCommand(query, sqlConnectionFilesManager);

            command.ExecuteNonQuery();

            sqlConnectionFilesManager.Close();
        }

        public List<int> LoadCoursesIDFromDB()
        {
            List<int> coursesID = new List<int>();
            string query = "SELECT * FROM CoursesID";

            sqlConnectionFilesManager.Open();

            command = new SqlCommand(query, sqlConnectionFilesManager);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                coursesID.Add((int)reader["id"]);
            }

            sqlConnectionFilesManager.Close();
            return coursesID;
        }

        public void SaveCoursesIDToDB(int course_id)
        {

            string query = "IF NOT EXISTS(SELECT * FROM CoursesID " +
                                            $"WHERE id = {course_id}) " +
                            "INSERT INTO CoursesID(id) " +
                            $"VALUES({course_id})";

            sqlConnectionFilesManager.Open();

            command = new SqlCommand(query, sqlConnectionFilesManager);

            command.ExecuteNonQuery();

            sqlConnectionFilesManager.Close();
        }

        public List<int> LoadFilesIDFromDB(int course_id)
        {
            List<int> FilesID = new List<int>();
            string query = $"SELECT * FROM FilesManager WHERE course_id={course_id}";

            sqlConnectionFilesManager.Open();

            command = new SqlCommand(query, sqlConnectionFilesManager);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                FilesID.Add((int)reader["files_id"]);
            }

            sqlConnectionFilesManager.Close();
            return FilesID;
        }

        public List<int> LoadDefinedFilesIDFromDB()
        {
            List<int> FilesID = new List<int>();
            string query = $"SELECT * FROM FilesManager";

            sqlConnectionFilesManager.Open();

            command = new SqlCommand(query, sqlConnectionFilesManager);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                FilesID.Add((int)reader["files_id"]);
            }

            sqlConnectionFilesManager.Close();
            return FilesID;
        }

        public void SaveLastFileIDToDB(int files_id)
        {
            string query = "DELETE FROM LastFileID " +
                $"INSERT INTO LastFileID(id) VALUES({files_id})";

            sqlConnectionFilesManager.Open();

            command = new SqlCommand(query, sqlConnectionFilesManager);

            command.ExecuteNonQuery();

            sqlConnectionFilesManager.Close();
        }

        public int LoadLastFilesIDFromDB()
        {
            string query = "SELECT * FROM LastFileID";

            sqlConnectionFilesManager.Open();

            command = new SqlCommand(query, sqlConnectionFilesManager);

            reader = command.ExecuteReader();

            int LastFileID = 3330000; // default value if sth went wrong

            while(reader.Read())
            {
                LastFileID = (int)reader["id"];
            }

            sqlConnectionFilesManager.Close();

            return LastFileID;
        }
    }
}
