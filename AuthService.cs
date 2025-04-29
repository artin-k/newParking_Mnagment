using Microsoft.Data.Sqlite;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfTest
{
    public class AuthService
    {
        private string _connectionString = "Data Source=parking.db";

        public void dbTests()
        {
            
            using var connection = new SqliteConnection("Data Source=parking.db");
            connection.Open();

            // Step 1: Get all table names
            var tableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%';";
            using var tableCommand = new SqliteCommand(tableQuery, connection);
            using var tableReader = tableCommand.ExecuteReader();

            var tableNames = new List<string>();
            while (tableReader.Read())
            {
                string tableName = tableReader.GetString(0);
                tableNames.Add(tableName);
            }

            foreach (var tableName in tableNames)
            {
                MessageBox.Show($"\n=== Table: {tableName} ===");

                // Step 2: Show columns for the table
                var columnCmd = connection.CreateCommand();
                columnCmd.CommandText = $"PRAGMA table_info({tableName});";
                using var columnReader = columnCmd.ExecuteReader();

                MessageBox.Show("Columns:");
                var columnNames = new List<string>();
                while (columnReader.Read())
                {
                    string name = columnReader.GetString(1);
                    string type = columnReader.GetString(2);
                    columnNames.Add(name);
                    MessageBox.Show($"- {name} ({type})");
                }

                // Step 3: Show data rows
                var dataCmd = connection.CreateCommand();
                dataCmd.CommandText = $"SELECT * FROM {tableName};";
                using var dataReader = dataCmd.ExecuteReader();

                MessageBox.Show("\nData:");
                if (!dataReader.HasRows)
                {
                    MessageBox.Show("(no rows)");
                }
                else
                {
                    while (dataReader.Read())
                    {
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            Console.Write($"{dataReader.GetName(i)}: {dataReader.GetValue(i)}  ");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }






        public void AddUser(string user, string pass)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string insertCmd = "INSERT INTO Users (Username, Password) VALUES (@user, @pass)";
            using var command = new SqliteCommand(insertCmd, connection);
            command.Parameters.AddWithValue("@user", user);
            command.Parameters.AddWithValue("@pass", pass);
            command.ExecuteNonQuery();
        }

        public bool Login(string user, string pass)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string selectCmd = "SELECT * FROM Users WHERE Username = @user AND Password = @pass";
            using var command = new SqliteCommand(selectCmd, connection);
            command.Parameters.AddWithValue("@user", user);
            command.Parameters.AddWithValue("@pass", pass);
            using var reader = command.ExecuteReader();
            return reader.Read();
        }
    }
}
