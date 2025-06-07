using Microsoft.Data.Sqlite;
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


namespace WpfTest
{
    public partial class manager_form : Window
    {
        public manager_form()
        {
            InitializeComponent();
            // This is what links to XAML

            List<Staff> allStaff = AuthService.GetStaffs();

            staffGrid.ItemsSource = allStaff;

            LoadFeeDataGrid();
        }



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

            LoadFeeDataGrid();
        }

        private void LoadFeeDataGrid()
        {
            AuthService authService = new AuthService();    
            var feeList = authService.ShowFee();
            feeDataGrid.ItemsSource = feeList;
        }


        private void setStatus_Click(object sender, RoutedEventArgs e)
        {
            parkingStatus parkingStatus = new parkingStatus();
            parkingStatus.Show();
        }

        private void setPayment_Click(object sender, RoutedEventArgs e)
        {
            AuthService authService = new AuthService();

            if (comboBoxVehicleType.SelectedItem is ComboBoxItem selectedItem)
            {
                string vehicleType = selectedItem.Content.ToString();
                if (int.TryParse(textBoxFee.Text, out int fee))
                {                    
                    authService.SetFee(vehicleType, fee);
                    MessageBox.Show("Fee updated successfully.");
                }
                else
                {
                    MessageBox.Show("Please enter a valid fee.");
                }
            }
            else
            {
                MessageBox.Show("First select the vehicle type");
            }
        }

        private void HideAllPages()
        {
            staffPanel.Visibility = Visibility.Hidden;
            staffGrid.Visibility = Visibility.Hidden;
            feeDataGrid.Visibility = Visibility.Hidden;
            setingFee.Visibility = Visibility.Hidden;
        }


        public void showStaff_Click(object sender, RoutedEventArgs e)
        {
            if (staffGrid.Visibility != Visibility.Visible)
            {
                HideAllPages();
                staffGrid.ItemsSource = AuthService.GetStaffs();
                staffGrid.Visibility = Visibility.Visible;
            }
            else
            {
                staffGrid.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (feeDataGrid.Visibility != Visibility.Visible)
            {
                HideAllPages();
                feeDataGrid.Visibility = Visibility.Visible;
            }
            else
            {
                feeDataGrid.Visibility = Visibility.Hidden;
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (setingFee.Visibility != Visibility.Visible)
            {
                HideAllPages();
                setingFee.Visibility = Visibility.Visible;

            }
            else
            {
                setingFee.Visibility = Visibility.Hidden;
            }
        }

        public void setStaffClick(object sender, RoutedEventArgs e)
        {
            if (staffPanel.Visibility != Visibility.Visible)
            {
                HideAllPages();
                staffPanel.Visibility = Visibility.Visible;

            }
            else
            {
                staffPanel.Visibility = Visibility.Hidden;
            }
        }
    }
        //
}

