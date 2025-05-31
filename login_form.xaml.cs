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
            authService.dbTests();

            TextDirectionHelper.ApplyToTextBox(userTxt);
            TextDirectionHelper.ApplyToPasswordBox(passTxt);
        }

        private void UserName_GotFocus(object sender, RoutedEventArgs e)
        {
            //if it any way user focus on the textbox the placeholder lost visibility
            usernamePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void userPlaceHolder(object sender, RoutedEventArgs e) 
        {
            usernamePlaceholder.Visibility = Visibility.Collapsed;
            userTxt.Focus();
        }

        private void passPlaceHolder(object sender, RoutedEventArgs e)
        {
            //if clicked on it it dissapear and focus on the textbox 
            passwordPlaceholder.Visibility = Visibility.Collapsed;
            passTxt.Focus();    
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordPlaceholder.Visibility = Visibility.Collapsed;
        }




        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string getUser = userTxt.Text;
            string getPass = passTxt.Password;

            if (string.IsNullOrEmpty(getUser) || !string.IsNullOrEmpty(getPass))
            {
                MessageBox.Show("fill up the textboxes carefully please");
                return;
            }
            else if (getUser == "admin" && getPass == "1234")
            {
                Admin_form af = new Admin_form();
                af.Show();
            }
            else
            {

            }


            //AuthService authService = new AuthService();
            //bool success = authService.Login(getUser, getPass);

            ///
            

            if (success)
            {
                MessageBox.Show("ورود با موفقیت انجام شد");
                var mf = new main_form(); //login for main form  
                mf.Show();

                this.Close(); 

            }
            else
            {
                MessageBox.Show("ابتدا ثبت نام کنید  ");
            }
               
        }
    }
}
