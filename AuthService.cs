using Microsoft.Data.Sqlite;
using System.Windows;
using System.Security.Cryptography;
using System.Text;
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

        private string Hash(string input)
        {
            //using sha256 for hash the users pass
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public void AddUser(string user, string pass)
        {
            var hashedUser = Hash(user);
            var hashedPass = Hash(pass);

            using var connection = new SqliteConnection("Data Source=parking.db");
            connection.Open();

            // 1. Check if user already exists
            string checkCmd = "SELECT COUNT(*) FROM Users WHERE Username = @user";
            using var checkCommand = new SqliteCommand(checkCmd, connection);
            checkCommand.Parameters.AddWithValue("@user", hashedUser);
            var result = checkCommand.ExecuteScalar();
            long userCount = (result != null) ? Convert.ToInt64(result) : 0;

            if (userCount > 0)
            {
                MessageBox.Show("این نام کاربری قبلاً ثبت شده است.");
                return;
            }

            // 2. Insert new user
            string insertCmd = "INSERT INTO Users (Username, Password) VALUES (@user, @pass)";
            using var insertCommand = new SqliteCommand(insertCmd, connection);
            insertCommand.Parameters.AddWithValue("@user", hashedUser);
            insertCommand.Parameters.AddWithValue("@pass", hashedPass);
            insertCommand.ExecuteNonQuery();
        }

        public bool Login(string user, string pass)
        {
            var hashedUser = Hash(user);
            var hashedPass = Hash(pass);
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string selectCmd = "SELECT * FROM Users WHERE Username = @user AND Password = @pass";
            using var command = new SqliteCommand(selectCmd, connection);
            command.Parameters.AddWithValue("@user", hashedUser);
            command.Parameters.AddWithValue("@pass", hashedPass);
            using var reader = command.ExecuteReader();
            return reader.Read();
        }
    }
}