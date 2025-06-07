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

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundVideo.Source = new Uri("backgrounds/login.mp4", UriKind.Relative);
            BackgroundVideo.Play();
        }


        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            BackgroundVideo.Position = TimeSpan.Zero;
            BackgroundVideo.Play();
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
            string getUser = txtUsername.Text.Trim();
            string getPass = passTxt.Password.Trim();


            if (string.IsNullOrWhiteSpace(getUser) || string.IsNullOrWhiteSpace(getPass))
            {
                MessageBox.Show("لطفاً نام کاربری و رمز عبور را وارد کنید");
                return;
            }

            // Admin login
            if (getUser == "admin" && getPass == "1234")
            {
                Admin_form af = new Admin_form();
                af.Show();
                
                return; 
            }

            AuthService authService = new AuthService();
            bool isManager = ManagerCheck.IsChecked == true;

            if (isManager)
            {
                if (authService.LoginAsManager(getUser, getPass))
                {
                    manager_form mf = new manager_form();
                    mf.Show();
                    this.Close();
                }
            }
            else
            {
                if (authService.LoginAsStaff(getUser, getPass)) 
                {
                    staff_form sf = new staff_form(); 
                    sf.Show();
                    this.Close();
                }
            }
        }

        private void txtUsername_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
