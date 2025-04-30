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
            DatabaseSetup.InitializeTables();
            AuthService authService = new AuthService();
           // authService.dbTests();
        }

        private void UserName_GotFocus(object sender, RoutedEventArgs e)
        {
            usernamePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordPlaceholder.Visibility = Visibility.Collapsed;
        }


        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e) 
        {
            var text = userTxt.Text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                // Check if the first character is Persian/Arabic
                char firstChar = text[0];
                if ((firstChar >= 0x0600 && firstChar <= 0x06FF) || // Arabic, Persian
                    (firstChar >= 0x0750 && firstChar <= 0x077F) || // Arabic Supplement
                    (firstChar >= 0xFB50 && firstChar <= 0xFDFF))   // Arabic Presentation Forms
                {
                    userTxt.FlowDirection = FlowDirection.RightToLeft;
                    
                }
                else
                {
                    userTxt.FlowDirection = FlowDirection.LeftToRight;
                    
                }
            }
            else
            {
                // Default direction when empty
                userTxt.FlowDirection = FlowDirection.LeftToRight;
                
            }
        }

        private void PasswordBox_TextChanged(object sender, RoutedEventArgs e)
        {
            string password = passTxt.Password;

            if (!string.IsNullOrWhiteSpace(password))
            {
                char firstChar = password[0];
                if ((firstChar >= 0x0600 && firstChar <= 0x06FF) ||    // Arabic / Persian
                    (firstChar >= 0x0750 && firstChar <= 0x077F) ||
                    (firstChar >= 0xFB50 && firstChar <= 0xFDFF))
                {
                    passTxt.FlowDirection = FlowDirection.RightToLeft;
                }
                else
                {
                    passTxt.FlowDirection = FlowDirection.LeftToRight;
                }
            }
            else
            {
                passTxt.FlowDirection = FlowDirection.LeftToRight;
            }
        }


        public void registerBtn_Click(object sender, MouseButtonEventArgs e)
        {
            string getUser = userTxt.Text;
            string getPass = passTxt.Password;


            AuthService authService = new AuthService();
            authService.AddUser(getUser, getPass);

            MessageBox.Show("کاربر با موفقیت ثبت شد");
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
