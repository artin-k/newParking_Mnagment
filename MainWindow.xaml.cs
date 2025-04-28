using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions

namespace WpfTest // ✅ Put namespace at top level
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            MessageBox.Show("moz1");
            string dbPath = "users.db";
      
                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();

                    string tableCmd = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY, Username TEXT, Password TEXT)\r\n";
                    //string tableCmd = "DROP TABLE Users";
                    using (var command = new SqliteCommand(tableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("moz");
            
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            
            string getUser = userTxt.Text;
            string getPass = passTxt.Text;

            if(getUser.length > 30 || getPass.length > 30){
                MessageBox.Show("too long username or password")
                return;
            }

            if(!IsStrongPassword(getPass)){
                MessageBox("must be at least 8 characters, include Upper,lower,digit,and special char");
            }

            AddUser(getUser, getPass);

            MessageBox.Show("User registered successfully");
        }

        void AddUser(string user, string pass)
        {
            using (var connection = new SqliteConnection("Data Source=users.db"))
            {
                connection.Open();
                string insertCmd = "INSERT INTO Users (Username, Password) VALUES (@user, @pass)";
                using (var command = new SqliteCommand(insertCmd, connection))
                {
                    command.Parameters.AddWithValue("@user", user);
                    command.Parameters.AddWithValue("@pass", pass);
                    command.ExecuteNonQuery();
                }
            }

        }

        public bool IsStrongPassword(string pass){
            if(pass.length > 8){
                return false;
            }

            bool hasUpper = Regex.IsMatch(pass, "[A-Z]");
            
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string getUser = userTxt.Text;
            string getPass = passTxt.Text;

            AuthService authService = new AuthService();
            bool success = authService.Login(getUser, getPass);

        }

        void loginFunc(string user, string pass)
        {
            using (var connection = new SqliteConnection("Data Source=users.db"))
            {
                connection.Open();

                // 🔹 Use a parameterized query to prevent SQL Injection
                string selectCmd = "SELECT * FROM Users WHERE Username = @user AND Password = @pass";

                using (var command = new SqliteCommand(selectCmd, connection))
                {
                    // 🔹 Add parameters properly
                    command.Parameters.AddWithValue("@user", user);
                    command.Parameters.AddWithValue("@pass", pass);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // 🔹 If there is a match, login is successful
                        {
                            MessageBox.Show("Find the banana successfully ");
                        }
                        else
                        {
                            MessageBox.Show("Username or password is wrong. Try again ❌");
                        }
                    }
                }
            }
        }
    }
}
