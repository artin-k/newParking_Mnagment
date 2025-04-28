using System.Windows;

namespace WpfTest // ✅ Put namespace at top level
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
