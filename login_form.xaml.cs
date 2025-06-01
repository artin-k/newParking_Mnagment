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
using System.Security.AccessControl;

namespace WpfTest 
{
    public partial class login_form : Window
    {
        public login_form()
        {
            InitializeComponent();
            //it sets up the database
            DatabaseSetup.InitializeTables();

            //a test of the database and the columns 
            AuthService authService = new AuthService();
            //authService.dbTests();

            TextDirectionHelper.ApplyToPasswordBox(passTxt);
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void passPlaceHolder(object sender, MouseButtonEventArgs e)
        {
            passTxt.Focus();
        }

        private void passTxt_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordPlaceholder.Visibility = string.IsNullOrEmpty(passTxt.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string getUser = txtUsername.Text;
            string getPass = passTxt.Password;

            
            if (getUser == "admin" && getPass == "1234")
            {
                Admin_form af = new Admin_form();
                af.Show();
            }
            else
            {

            }

               
        }
    }
}
