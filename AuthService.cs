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

        public bool LoginAsManager(string user, string pass)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();
                string selectCmd = "SELECT * FROM Manager";
                using var command = new SqliteCommand(selectCmd, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string? dbUser = reader["NationalCode"].ToString();
                    string? dbPass = reader["Password"].ToString();
                    if (dbUser == user && dbPass == pass)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("an error occured: " + ex.Message);
                return false;
            }
        }

        public bool LoginAsStaff(string user, string pass)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();
                string selectCmd = "SELECT * FROM Staff";
                using var command = new SqliteCommand(selectCmd, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string? dbUser = reader["NationalCode"].ToString();
                    string? dbPass = reader["Password"].ToString();
                    if (dbUser == user && dbPass == pass)
                        return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("an error occured: " + ex.Message);
                return false;
            }
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

        public bool setManager(string name, string user, string pass)
        {
            try
            {
                var hashedUser = Hash(user);
                var hashedPass = Hash(pass);

                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();

                string checkCmd = "SELECT COUNT(*) FROM Manager WHERE NationalCode = @user";
                using var checkCommand = new SqliteCommand(checkCmd, connection);
                checkCommand.Parameters.AddWithValue("@user", hashedUser);
                var result = checkCommand.ExecuteScalar();
                long userCount = (result != null) ? Convert.ToInt64(result) : 0;

                if (userCount > 0)
                {
                    MessageBox.Show("این مدیر قبلاً ثبت شده است.");
                    return false;
                }

                string insertCmd = "INSERT INTO Manager (Name, Password, NationalCode, Role, ParkingFee) VALUES (@name, @pass, @user, @role, @fee)";
                using var insertCommand = new SqliteCommand(insertCmd, connection);

                insertCommand.Parameters.AddWithValue("@name", name);
                insertCommand.Parameters.AddWithValue("@pass", hashedPass);
                insertCommand.Parameters.AddWithValue("@user", hashedUser);
                insertCommand.Parameters.AddWithValue("@role", "manager");
                insertCommand.Parameters.AddWithValue("@fee", 0);
                insertCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show("an error occured " + ex.Message);
                return false;   
            }
        }

        public static void SetParkingPayment(int pay)
        {
            using var connection = new SqliteConnection("Data Source=parking.db");
            connection.Open();

            // Check if any row exists in Manager table
            string checkCommand = "SELECT COUNT(*) FROM Manager";
            using var checkCmd = new SqliteCommand(checkCommand, connection);
            object? result = checkCmd.ExecuteScalar();
            int count = result != null ? Convert.ToInt32(result) : 0;
           
            if (count > 0)
            {
                // Update existing row
                string updateCommand = "UPDATE Manager SET ParkingFee = @fee";
                using var updateCmd = new SqliteCommand(updateCommand, connection);
                updateCmd.Parameters.AddWithValue("@fee", pay);
                updateCmd.ExecuteNonQuery();
            }
            else
            {
                // Insert new row
                string insertCommand = "INSERT INTO Manager (ParkingFee) VALUES (@fee)";
                using var insertCmd = new SqliteCommand(insertCommand, connection);
                insertCmd.Parameters.AddWithValue("@fee", pay);
                insertCmd.ExecuteNonQuery();
            }
        }


        public static int ShowPayment()
        {
            int payment = 0;
            using var connection = new SqliteConnection("Data Source=parking.db");
            connection.Open();

            string commandText = "SELECT ParkingFee FROM Manager LIMIT 1";
            using var command = new SqliteCommand(commandText, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                payment = Convert.ToInt32(reader["ParkingFee"]);
            }

            return payment;
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
                string insertCmd = "INSERT INTO Cars (ParkPlace, PhoneNumber, Specification, EntryTime, Plate, Date, IsExited) " +
                       "VALUES (@parkPlace, @phoneNum, @CarSepc, @entrytime, @plate, @date, @IsExited)";

                using var insertCommand = new SqliteCommand(insertCmd, connection);
                insertCommand.Parameters.AddWithValue("@parkPlace", car.ParkPlace);
                insertCommand.Parameters.AddWithValue("@phoneNum", car.PhoneNumber);               
                insertCommand.Parameters.AddWithValue("@CarSepc", car.Specification);
                insertCommand.Parameters.AddWithValue("@entrytime", car.EntryTime);
                insertCommand.Parameters.AddWithValue("@plate", car.Plate);
                insertCommand.Parameters.AddWithValue("@date", car.Date);
                insertCommand.Parameters.AddWithValue("@IsExited", car.IsExited);
                insertCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }

        public static List<Car> GetCars()
        {
            var cars = new List<Car>();
            using (var connection = new SqliteConnection("Data Source=parking.db"))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT * FROM Cars WHERE IsExited = false", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var car = new Car(
                            reader["Plate"]?.ToString()?? "",
                            reader["Specification"]?.ToString()?? "",
                            reader["PhoneNumber"]?.ToString()?? "",
                            reader["ParkPlace"]?.ToString()?? "",
                            reader["EntryTime"]?.ToString()?? "",
                            reader["ExitTime"]?.ToString()?? "",
                            Convert.ToBoolean(reader["IsExited"])
                        );
                        cars.Add(car);
                    }
                }
            }
            return cars;
        }

        public static double ExitCar(string plateNumber)
        {

            try
            {
                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();

                // Step 1: Update the exit time
                var updateCommand = new SqliteCommand(
                    "UPDATE Cars SET ExitTime = @exitTime, IsExited = 1 WHERE Plate = @plate", connection);
                updateCommand.Parameters.AddWithValue("@exitTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                updateCommand.Parameters.AddWithValue("@plate", plateNumber);
                updateCommand.ExecuteNonQuery();

                // Step 2: Read updated entry/exit time
                var selectCommand = new SqliteCommand(
                    "SELECT EntryTime, ExitTime FROM Cars WHERE Plate = @plate", connection);
                selectCommand.Parameters.AddWithValue("@plate", plateNumber);

                using var reader = selectCommand.ExecuteReader();

                if (reader.Read())
                {
                    string? entryStr = reader["EntryTime"]?.ToString();
                    string? exitStr = reader["ExitTime"]?.ToString();

                    if (DateTime.TryParse(entryStr, out DateTime entry) &&
                        DateTime.TryParse(exitStr, out DateTime exit))
                    {
                        var duration = exit - entry;
                        int feePerHour = ShowPayment(); // get from Manager table
                        return Math.Ceiling(duration.TotalHours) * feePerHour;
                    }
                }

                return 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show("an error occured" +  ex.Message);
                return -1;
            }

        }
    }
}