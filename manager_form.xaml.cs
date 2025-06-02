using Microsoft.VisualBasic;
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

//add code meli to staff 

namespace WpfTest
{
    public partial class manager_form : Window
    {

        public manager_form()
        {

            InitializeComponent();

            List<Staff> allStaff = AuthService.GetStaffs();

            staffGrid.ItemsSource = allStaff;

            paymentLable();
        }

        public void paymentLable()
        {
            int payemnt = AuthService.ShowPayment();
            feeLable.Content = payemnt;
        }

        public void setStaffClick(object sender, RoutedEventArgs e)
        {
            staffPanel.Visibility = staffPanel.Visibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;
        }
        //

        public void saveStaff_click(object sender, RoutedEventArgs e) 
        {
            string fullName = txtFullName.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            string role = comboRole.Text;
            string phone = txtPhone.Text;           
            DateTime? joinDate = dpJoinDate.SelectedDate;
            if (joinDate.HasValue)
            {
                string formattedDate = joinDate.Value.ToString("yyyy-MM-dd"); 
            }

            bool anyEmptyField = new[] { fullName, username, password, role, phone }.Any(string.IsNullOrWhiteSpace);           

            if (anyEmptyField || !joinDate.HasValue)
            {
                MessageBox.Show("لطفاً تمام فیلدها را کامل پر کنید.");               
                return;
            }

            Staff st = new Staff(fullName, username, password, role, phone, joinDate.Value);
            AuthService authService = new AuthService();
            authService.AddStaff(st);
        }

        private void setStatus_Click(object sender, RoutedEventArgs e)
        {
            parkingStatus parkingStatus = new parkingStatus();
            parkingStatus.Show();
        }

        private void setPayment_Click(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox("Enter paying fee for your parking:", "Payment Input", "");

            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int fee))
            {
                AuthService.SetParkingPayment(fee);
                paymentLable();
            }
            else
            {
                MessageBox.Show("Please enter a valid number.");
            }
        }

        public void showStaff_Click(object sender, RoutedEventArgs e)
        {
            if (staffGrid.Visibility != Visibility.Visible)
            {
                staffGrid.ItemsSource = AuthService.GetStaffs();
                staffGrid.Visibility = Visibility.Visible;
            }
            else
            {
                staffGrid.Visibility = Visibility.Collapsed;
            }
        }


    }
}

