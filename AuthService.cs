using Microsoft.Data.Sqlite;

namespace WpfTest
{
    public class AuthService
    {
        private string _connectionString = "Data Source=users.db";

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
