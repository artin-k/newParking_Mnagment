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

namespace WpfTest 
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

            if (success)
                MessageBox.Show("Login successful!");
            else
                MessageBox.Show("Login failed.");
        }
    }
}
