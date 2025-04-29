using Microsoft.Data.Sqlite;

namespace WpfTest
{
    public static class DatabaseSetup
    {
        private static string _connectionString = "Data Source=parking.db";

        public static void InitializeTables()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();


            // Create Users table if it doesn't exist
            var createUsers = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL,
                    Password TEXT NOT NULL
                );
            ", connection);
            createUsers.ExecuteNonQuery();


            // Create Cars table if it doesn't exist
            var createCars = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Cars (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CarNum TEXT,
                    Plate TEXT,
                    Specification TEXT,
                    Model TEXT,
                    Color TEXT,
                    PhoneNumber TEXT,
                    ParkPlace TEXT,
                    EntryTime DATETIME,
                    ExitTime DATETIME
                );
            ", connection);
            createCars.ExecuteNonQuery();

            // Create Users table if it doesn't exist
            var createStaff = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Staff (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,          
                    Role TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL,
                    JoinDate DATE NOT NULL
                );
            ", connection);
            createUsers.ExecuteNonQuery();
        }
    }
}
