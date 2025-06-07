using Microsoft.Data.Sqlite;
using System.Windows;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Policy;
using System.Xml.Linq;

namespace WpfTest
{
    public class AuthService
    {

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

        private static string Hash(string input)
        {
            //using sha256 for hash the users pass
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool LoginAsManager(string user, string pass)
        {
            var hashedUser = Hash(user);
            var hashedPass = Hash(pass);
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
                    if (dbUser == hashedUser && dbPass == hashedPass)
                    {
                        string? name = reader["Name"].ToString();
                        MessageBox.Show($"welcome {name} as manager ");
                        return true;
                    }

                }

                MessageBox.Show("manager doesn't exist");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return false;
            }
        }


        public bool LoginAsStaff(string user, string pass)
        {
            var hashedUser = Hash(user);
            var hashedPass = Hash(pass);
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
                    if (dbUser == hashedUser && dbPass == hashedPass)
                    {
                        string? name = reader["Name"].ToString();
                        MessageBox.Show($"welcome {name} as staff ");
                        return true;
                    }

                      
                }
                MessageBox.Show("log in as staff failed");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("an error occured: " + ex.Message);
                return false;
            }
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

                string insertCmd = "INSERT INTO Manager (Name, Password, NationalCode, Role) VALUES (@name, @pass, @user, @role)";
                using var insertCommand = new SqliteCommand(insertCmd, connection);

                insertCommand.Parameters.AddWithValue("@name", name);
                insertCommand.Parameters.AddWithValue("@pass", hashedPass);
                insertCommand.Parameters.AddWithValue("@user", hashedUser);
                insertCommand.Parameters.AddWithValue("@role", "manager");
                insertCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex) 
            {
                MessageBox.Show("an error occured " + ex.Message);
                return false;   
            }
        }

        public void SetFee(string vehicleType, int fee)
        {
            using var connection = new SqliteConnection("Data Source=parking.db");
            connection.Open();

            var cmd = new SqliteCommand(@"
                INSERT INTO VehicleTypeFee (VehicleType, FeePerHour)
                VALUES (@type, @fee)
                ON CONFLICT(VehicleType) DO UPDATE SET FeePerHour = excluded.FeePerHour;
            ", connection);

            cmd.Parameters.AddWithValue("@type", vehicleType);
            cmd.Parameters.AddWithValue("@fee", fee);

            cmd.ExecuteNonQuery();
        }

        public List<VehicleFee> ShowFee()
        {
            var feeList = new List<VehicleFee>();

            using (var connection = new SqliteConnection("Data Source=parking.db"))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT * FROM VehicleTypeFee", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var vehicleType = reader["VehicleType"]?.ToString() ?? "";
                        var feeStr = reader["FeePerHour"]?.ToString() ?? "0";
                        int fee = int.TryParse(feeStr, out int result) ? result : 0;

                        feeList.Add(new VehicleFee
                        {
                            VehicleType = vehicleType,
                            Fee = fee
                        });
                    }
                }
            }

            return feeList;
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
                insertCommand.Parameters.AddWithValue("@name", st.Name);
                insertCommand.Parameters.AddWithValue("@pass", hashedPass);
                insertCommand.Parameters.AddWithValue("@NationalCode", hashedNaCode);
                insertCommand.Parameters.AddWithValue("@role", st.Role);
                insertCommand.Parameters.AddWithValue("@salary", st.Salary);
                insertCommand.Parameters.AddWithValue("@phone", st.PhoneNumber);
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
                string insertCmd = "INSERT INTO Cars (ParkPlace, PhoneNumber, Specification, EntryTime, Plate, Date, IsExited, VehicleType, Fee) " +
                                   "VALUES (@parkPlace, @phoneNum, @CarSepc, @entrytime, @plate, @date, @IsExited, @vehicleType, @fee)";

                using var insertCommand = new SqliteCommand(insertCmd, connection);
                insertCommand.Parameters.AddWithValue("@parkPlace", car.ParkPlace);
                insertCommand.Parameters.AddWithValue("@phoneNum", car.PhoneNumber);
                insertCommand.Parameters.AddWithValue("@vehicleType", car.VehicleType);
                insertCommand.Parameters.AddWithValue("@fee", car.Fee);
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

        public bool UpdateCar(Car car)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();

                // Step 3: Free the parking spot
              /*  if (!string.IsNullOrWhiteSpace(parkPlace) && int.TryParse(parkPlace, out int parkPlaceId))
                {
                    var freeSpotCommand = new SqliteCommand(
                        "UPDATE ParkingSpots SET IsOccupied = 0 WHERE Id = @id", connection);
                    freeSpotCommand.Parameters.AddWithValue("@id", parkPlaceId);
                    freeSpotCommand.ExecuteNonQuery();
                }*/


                string updateCmd = @"UPDATE Cars 
                             SET ParkPlace = @parkPlace, 
                                 PhoneNumber = @phoneNum,
                                 Specification = @CarSpec,
                                 EntryTime = @entrytime,
                                 Date = @date,
                                 IsExited = @IsExited,
                                 VehicleType = @vehicleType,
                                 Fee = @fee
                             WHERE Plate = @plate";

                using var updateCommand = new SqliteCommand(updateCmd, connection);
                updateCommand.Parameters.AddWithValue("@parkPlace", car.ParkPlace);
                updateCommand.Parameters.AddWithValue("@phoneNum", car.PhoneNumber);
                updateCommand.Parameters.AddWithValue("@CarSpec", car.Specification);
                updateCommand.Parameters.AddWithValue("@entrytime", car.EntryTime);
                updateCommand.Parameters.AddWithValue("@date", car.Date);
                updateCommand.Parameters.AddWithValue("@IsExited", car.IsExited);
                updateCommand.Parameters.AddWithValue("@vehicleType", car.VehicleType);
                updateCommand.Parameters.AddWithValue("@fee", car.Fee);
                updateCommand.Parameters.AddWithValue("@plate", car.Plate); // Plate = unique key

                int rowsAffected = updateCommand.ExecuteNonQuery();

                // Step 2: Free all spots
                using var freeAll = new SqliteCommand("UPDATE ParkingSpots SET IsOccupied = 0", connection);
                freeAll.ExecuteNonQuery();

                // Step 3: Get all active cars (IsExited = 0)
                var getActiveCars = new SqliteCommand("SELECT ParkPlace FROM Cars WHERE IsExited = 0", connection);
                using var reader = getActiveCars.ExecuteReader();
                while (reader.Read())
                {
                    string? placeStr = reader["ParkPlace"]?.ToString();
                    if (int.TryParse(placeStr, out int parkId))
                    {
                        var occupyCmd = new SqliteCommand("UPDATE ParkingSpots SET IsOccupied = 1 WHERE Id = @id", connection);
                        occupyCmd.Parameters.AddWithValue("@id", parkId);
                        occupyCmd.ExecuteNonQuery();
                    }
                }
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while updating car: {ex.Message}");
                return false;
            }
        }

        public bool DeleteCar(string plate)
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=parking.db");
                connection.Open();

                // ── 1.  Find the park-place for this car
                var getPlaceCmd = new SqliteCommand(
                    "SELECT ParkPlace FROM Cars WHERE Plate = @plate", connection);
                getPlaceCmd.Parameters.AddWithValue("@plate", plate);

                var parkPlaceObj = getPlaceCmd.ExecuteScalar();
                if (parkPlaceObj != null && int.TryParse(parkPlaceObj.ToString(), out int placeId))
                {
                    // ── 2.  Free that spot
                    var freeSpotCmd = new SqliteCommand(
                        "UPDATE ParkingSpots SET IsOccupied = 0 WHERE Id = @id", connection);
                    freeSpotCmd.Parameters.AddWithValue("@id", placeId);
                    freeSpotCmd.ExecuteNonQuery();
                }

                // ── 3.  Delete the car row
                var delCmd = new SqliteCommand(
                    "DELETE FROM Cars WHERE Plate = @plate", connection);
                delCmd.Parameters.AddWithValue("@plate", plate);

                return delCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete error: {ex.Message}");
                return false;
            }
        }


        public static List<Car> GetCars()
        {
            var cars = new List<Car>();
            using (var connection = new SqliteConnection("Data Source=parking.db"))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT * FROM Cars", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var car = new Car(
                            reader["Plate"]?.ToString() ?? "",
                            reader["Specification"]?.ToString() ?? "",
                            reader["PhoneNumber"]?.ToString() ?? "",
                            reader["VehicleType"]?.ToString() ?? "",
                            reader["ParkPlace"]?.ToString() ?? "",
                            reader["EntryTime"]?.ToString() ?? "",
                            reader["ExitTime"]?.ToString() ?? "",
                            Convert.ToBoolean(reader["IsExited"])
                        );
                        car.Fee = Convert.ToInt32(reader["Fee"]);
                        cars.Add(car);
                    }
                }
            }
            return cars;
        }

        public static List<Manager> GetManagers()
        {
            var Managers = new List<Manager>();
            using (var connection = new SqliteConnection("Data Source=parking.db"))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT * FROM Manager", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var manager = new Manager
                        {
                            Name = reader["Name"]?.ToString() ?? "",
                            NationalCode = reader["NationalCode"]?.ToString() ?? "",
                            Password = reader["Password"]?.ToString() ?? ""
                        };
                        Managers.Add(manager);
                    }
                }
            }
            return Managers;
        }



        public static List<Staff> GetStaffs() 
        {           
            var staffs = new List<Staff>();
            using (var connection = new SqliteConnection("Data Source = parking.db"))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT * FROM Staff", connection);               
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime.TryParse(reader["JoinDate"]?.ToString(), out DateTime joinDate);
                        var hasheduser = Hash(reader["NationalCode"]?.ToString() ?? "");
                        var hashedpass = Hash(reader["Password"]?.ToString() ?? "");


                        var staff = new Staff(
                            reader["Name"]?.ToString() ?? "",
                            hashedpass,
                            hasheduser,
                            reader["Role"]?.ToString() ?? "",                            
                            reader["PhoneNumber"]?.ToString() ?? "",
                            joinDate
                            );
                        staffs.Add(staff);
                    }
                }                
            }
            return staffs;
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

                // Step 2: Read updated entry/exit time and vehicle type
                var selectCommand = new SqliteCommand(
                    "SELECT EntryTime, ExitTime, ParkPlace, VehicleType FROM Cars WHERE Plate = @plate", connection);
                selectCommand.Parameters.AddWithValue("@plate", plateNumber);

                using var reader = selectCommand.ExecuteReader();
                if (reader.Read())
                {
                    string? parkPlace = reader["ParkPlace"]?.ToString();
                    string? entryStr = reader["EntryTime"]?.ToString();
                    string? exitStr = reader["ExitTime"]?.ToString();
                    string? vehicleType = reader["VehicleType"]?.ToString();

                    if (DateTime.TryParse(entryStr, out DateTime entry) &&
                        DateTime.TryParse(exitStr, out DateTime exit) &&
                        !string.IsNullOrWhiteSpace(vehicleType))
                    {
                        // Step 3: Free the parking spot
                        if (!string.IsNullOrWhiteSpace(parkPlace) && int.TryParse(parkPlace, out int parkPlaceId))
                        {
                            var freeSpotCommand = new SqliteCommand(
                                "UPDATE ParkingSpots SET IsOccupied = 0 WHERE Id = @id", connection);
                            freeSpotCommand.Parameters.AddWithValue("@id", parkPlaceId);
                            freeSpotCommand.ExecuteNonQuery();
                        }

                        // Step 4: Get FeePerHour from VehicleTypeFee table
                        int feePerHour = 0;
                        var feeCommand = new SqliteCommand(
                            "SELECT FeePerHour FROM VehicleTypeFee WHERE VehicleType = @type", connection);
                        feeCommand.Parameters.AddWithValue("@type", vehicleType);

                        object? feeResult = feeCommand.ExecuteScalar();
                        if (feeResult != null && int.TryParse(feeResult.ToString(), out int fee))
                        {
                            feePerHour = fee;
                        }
                        else
                        {
                            // fallback/default fee if vehicle type not found in DB
                            feePerHour = 5000;
                        }

                        // Step 5: Calculate time difference
                        var duration = exit - entry;
                        var totalHours = Math.Ceiling(duration.TotalHours);
                        double totalFee = totalHours * feePerHour;

                        // Step 6: Update the fee in the Cars table
                        var updateFeeCmd = new SqliteCommand(
                            "UPDATE Cars SET Fee = @fee WHERE Plate = @plate", connection);
                        updateFeeCmd.Parameters.AddWithValue("@fee", totalFee);
                        updateFeeCmd.Parameters.AddWithValue("@plate", plateNumber);
                        updateFeeCmd.ExecuteNonQuery();

                        return totalFee;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return -1;
            }
        }


    }
}