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
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace WpfTest 
{
    public partial class login_form : Window
    {
        public login_form()
        {
            InitializeComponent();
            DatabaseSetup.InitializeTables();
            AuthService authService = new AuthService();
            //authService.dbTests();
        }
        
        public void registerBtn_Click(object sender, EventArgs e)
        {
            string getUser = userTxt.Text;
            string getPass = passTxt.Text;

            AuthService authService = new AuthService();
            authService.AddUser(getUser, getPass);

            MessageBox.Show("User registered successfully");
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string getUser = userTxt.Text;
            string getPass = passTxt.Text;

            AuthService authService = new AuthService();
            bool success = authService.Login(getUser, getPass);

            ///
            

            if (success)
            {
                MessageBox.Show("Login successful!");
                var mf = new main_form(); //login for staff form  
                mf.Show();

                this.Close(); 

            }
            else
            {
                MessageBox.Show("Login failed.");
            }
               
        }
    }
}
