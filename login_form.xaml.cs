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

        public void registerBtn_Click(object sender, MouseButtonEventArgs e)
        {
            //get the data from textboxes 
            string getUser = userTxt.Text;
            string getPass = passTxt.Password;

            //make sure the users pass is good 
            if (IsStrongPassword(getPass))
            {
                //building a object from auth cass and use the add user method
                AuthService authService = new AuthService();
                authService.AddUser(getUser, getPass);

                MessageBox.Show("کاربر با موفقیت ثبت شد");
            }
            else
            {
                MessageBox.Show("رمز عبور باید حداقل ۸ کاراکتر، شامل حروف بزرگ و کوچک، عدد و یک کاراکتر خاص باشد.");
            }          
        }

        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            var hasMinimum8Chars = password.Length >= 8;
            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            return hasMinimum8Chars && hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }


        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string getUser = userTxt.Text;
            string getPass = passTxt.Password;

            AuthService authService = new AuthService();
            bool success = authService.Login(getUser, getPass);

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
