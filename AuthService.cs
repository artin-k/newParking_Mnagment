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

        public void AddStaff(Staff st)
        {
            try
            {
                var hashedNaCode = Hash(st.NationalCode);
                var hashedPass = Hash(st.Password);

                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();

                string checkCmd = "SELECT COUNT(*) FROM Staff WHERE NationalCode = @NationalCode";
                using var checkCommand = new SqliteCommand(checkCmd, connection);
                checkCommand.Parameters.AddWithValue("@NationalCode", hashedNaCode);
                var result = checkCommand.ExecuteScalar();
                long userCount = (result != null) ? Convert.ToInt64(result) : 0;

                if (userCount > 0)
                {
                    MessageBox.Show("این نام کاربری قبلاً ثبت شده است.");
                    return;
                }

                //  Insert new staff
                string insertCmd = "INSERT INTO Staff (Name, Password, NationalCode, Role, Salary, PhoneNumber, JoinDate) " +
                       "VALUES (@name, @pass, @NationalCode, @role, @salary, @phone, @date)";

                using var insertCommand = new SqliteCommand(insertCmd, connection);
                insertCommand.Parameters.AddWithValue("@name", st.FullName);
                insertCommand.Parameters.AddWithValue("@pass", hashedPass);
                insertCommand.Parameters.AddWithValue("@NationalCode", hashedNaCode);
                insertCommand.Parameters.AddWithValue("@role", st.Role);
                insertCommand.Parameters.AddWithValue("@salary", st.salary);
                insertCommand.Parameters.AddWithValue("@phone", st.Phone);
                insertCommand.Parameters.AddWithValue("@date", st.JoinDate);
                insertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }


        }

        public static List<ParkingSpot> GetAllSpots()
        {
            var spots = new List<ParkingSpot>();

            using var connection = new SqliteConnection("Data Source=parking.db");
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, IsOccupied FROM ParkingSpots";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        spots.Add(new ParkingSpot
                        {
                            Id = reader.GetInt32(0),
                            IsOccupied = reader.GetBoolean(1)
                        });
                    }
                }
            }

            return spots;
        }

        public static ParkingSpot? GetFirstEmptySpot()
        {
            using (var conn = new SqliteConnection("Data Source=parking.db"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Id FROM ParkingSpots WHERE IsOccupied = 0 LIMIT 1";

                using (var reader = cmd.ExecuteReader())
                {
                    MessageBox.Show("Checking for empty spot...");
                    if (reader.Read())
                    {
                        Console.WriteLine("Free spot found: " + reader.GetInt32(0));
                        return new ParkingSpot
                        {
                            Id = reader.GetInt32(0),
                            IsOccupied = false
                        };
                    }
                    else
                    {
                        MessageBox.Show("No free spots found.");
                    }
                }
            }

            return null; // No free spots
        }


        public static void UpdateSpotStatus(int spotId, bool isOccupied)
        {
            using (var conn = new SqliteConnection("Data Source=parking.db"))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE ParkingSpots SET IsOccupied = @status WHERE Id = @id";
                cmd.Parameters.AddWithValue("@status", isOccupied ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", spotId);
                cmd.ExecuteNonQuery();
            }
        }

        public bool registerCar(Car car)
        {
            

            try
            {

                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();

                string checkCmd = "SELECT COUNT(*) FROM Cars WHERE plate = @plate";
                using var checkCommand = new SqliteCommand(checkCmd, connection);
                checkCommand.Parameters.AddWithValue("@plate",car.Plate);
                var result = checkCommand.ExecuteScalar();
                long carCount = (result != null) ? Convert.ToInt64(result) : 0;

                if (carCount > 0)
                {
                    MessageBox.Show("این نام خودرو قبلاً ثبت شده است.");
                    return false;
                }

                //  Insert new staff
                string insertCmd = "INSERT INTO Cars (ParkPlace, PhoneNumber, Specification, EntryTime, Plate, Date) " +
                       "VALUES (@parkPlace, @phoneNum, @CarSepc, @entrytime, @plate, @date)";

                using var insertCommand = new SqliteCommand(insertCmd, connection);
                insertCommand.Parameters.AddWithValue("@parkPlace", car.ParkPlace);
                insertCommand.Parameters.AddWithValue("@phoneNum", car.PhoneNumber);               
                insertCommand.Parameters.AddWithValue("@CarSepc", car.Specification);
                insertCommand.Parameters.AddWithValue("@entrytime", car.EntryTime);
                insertCommand.Parameters.AddWithValue("@plate", car.Plate);
                insertCommand.Parameters.AddWithValue("@date", car.Date);
                insertCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

    }
}