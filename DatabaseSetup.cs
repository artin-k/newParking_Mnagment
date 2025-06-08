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
           
                        // Create Cars table if it doesn't exist
            var createCars = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS Cars (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Plate TEXT NOT NULL,
                    Specification TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL,
                    VehicleType TEXT NOT NULL,       
                    ParkPlace TEXT NOT NULL,
                    EntryTime TEXT NOT NULL,
                    ExitTime TEXT,
                    IsExited BOOLEAN NOT NULL,
                    Date TEXT NOT NULL,
                    Fee INTEGER DEFAULT 0
                );
                ", connection);
            createCars.ExecuteNonQuery();



            // Create Users table if it doesn't exist
            //national code stand for username 
            var createStaff = new SqliteCommand(@"
                    CREATE TABLE IF NOT EXISTS Staff (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,                    
                    Name TEXT NOT NULL,  
                    Password TEXT NOT NULL DEFAULT '',
                    NationalCode TEXT NOT NULL DEFAULT '',
                    Role TEXT NOT NULL,
                    Salary TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL,
                    JoinDate DATE NOT NULL                   
                );
            ", connection);
            createStaff.ExecuteNonQuery();


            var createManager = new SqliteCommand(@"
                    CREATE TABLE IF NOT EXISTS Manager (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,                    
                    Name TEXT NOT NULL,  
                    Password TEXT NOT NULL DEFAULT '',
                    NationalCode TEXT NOT NULL DEFAULT '',
                    Role TEXT NOT NULL
                );
            ", connection);            
            createManager.ExecuteNonQuery();


            var VehicleTypeFee = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS VehicleTypeFee (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    VehicleType TEXT NOT NULL UNIQUE,  
                    FeePerHour INTEGER NOT NULL 0
                );
            ", connection);
            VehicleTypeFee.ExecuteNonQuery();


            var createParkPlace = new SqliteCommand(@"
                CREATE TABLE IF NOT EXISTS ParkingSpots (
                    Id INTEGER PRIMARY KEY,
                    IsOccupied BOOLEAN NOT NULL
                );
            ", connection);
            createParkPlace.ExecuteNonQuery();


            // Seed 30 spots if not exists
            for (int i = 0; i < 30; i++)
            {
                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = "SELECT COUNT(*) FROM ParkingSpots WHERE Id = $id";
                checkCmd.Parameters.AddWithValue("$id", i);

                object? result = checkCmd.ExecuteScalar();
                long count = (result as long?) ?? 0;

                if (count == 0)
                {
                    var insertCmd = connection.CreateCommand();
                    insertCmd.CommandText = "INSERT INTO ParkingSpots (Id, IsOccupied) VALUES ($id, $isOccupied)";
                    insertCmd.Parameters.AddWithValue("$id", i);
                    insertCmd.Parameters.AddWithValue("$isOccupied", false);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }


    }
}

