using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for staff_form.xaml
    /// </summary>
    public partial class manager_form : Window
    {
        public manager_form()
        {
            InitializeComponent();
        }

        public void setStaffClick(object sender, RoutedEventArgs e)
        {
            staffPanel.Visibility = staffPanel.Visibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;
        }
        //

        public void ConfirmStaffClick(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string role = txtRole.Text;
            string phone = txtPhone.Text;
            string joinDate = txtJoinDate.Text;

            // Check if ANY string is null/empty
            bool anyInvalid = new[] { fullName, username, password, role, phone, joinDate }.Any(s => string.IsNullOrEmpty(s));
            Console.WriteLine($"Any invalid: {anyInvalid}"); 
        }
    }
}

