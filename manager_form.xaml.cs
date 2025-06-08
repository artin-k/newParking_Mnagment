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

        public bool IsAllDigit(string str)
        {
            return str.All(char.IsDigit);
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && IsAllDigit(phoneNumber) && phoneNumber.Length == 11;
        }

        public void saveStaff_click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string username = txtUsername.Text; 
            string password = txtPassword.Text;
            string role = comboRole.Text;
            string phone = txtPhone.Text;
            DateTime? joinDate = dpJoinDate.SelectedDate;



            bool anyEmptyField = new[] { fullName, username, password, role, phone }.Any(string.IsNullOrWhiteSpace);


            if (anyEmptyField || !joinDate.HasValue || !IsPhoneNumberValid(phone) || !(IsAllDigit(username) && username.Length != 9))
            {
                MessageBox.Show("لطفاً تمام فیلدها با دقت کامل پر کنید.");
                return;
            }

            Staff st = new Staff(fullName, password, username, role, phone, joinDate.Value);
            AuthService authService = new AuthService();
            authService.AddStaff(st);

            MessageBox.Show("کارمند با موفقیت ذخیره شد");
            LoadFeeDataGrid(); 
        }


        public void CancelBtnـClick(object sender, RoutedEventArgs e)
        {
            txtFullName.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            comboRole.SelectedIndex = 0;
            txtPhone.Text = string.Empty ;
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
                string? vehicleType = selectedItem.Content.ToString();
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
            feeDataStack.Visibility = Visibility.Hidden;
            setingFee.Visibility = Visibility.Hidden;
            gridOptions.Visibility = Visibility.Hidden;
        }


        public void showStaff_Click(object sender, RoutedEventArgs e)
        {
            if (staffGrid.Visibility != Visibility.Visible)
            {
                HideAllPages();
                staffGrid.ItemsSource = AuthService.GetStaffs();
                staffGrid.Visibility = Visibility.Visible;
                gridOptions.Visibility = Visibility.Visible;
            }
            else
            {
                staffGrid.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (feeDataStack.Visibility != Visibility.Visible)
            {
                HideAllPages();
                feeDataStack.Visibility = Visibility.Visible;
            }
            else
            {
                feeDataStack.Visibility = Visibility.Hidden;
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

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            var staffs = (List<Staff>)staffGrid.ItemsSource;
            AuthService authService = new AuthService();
            int successCount = 0;

            foreach (var staff in staffs)
            {
                if (authService.UpdateStaff(staff)) 
                {
                    successCount++;
                }
            }

            MessageBox.Show($"{successCount} staffs updated successfully.");
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            AuthService auth = new AuthService();

            if (staffGrid.SelectedItem is Staff selectedStaff)
            {
                var confirm = MessageBox.Show(
                    $"Delete staff with phone number {selectedStaff.PhoneNumber}?",
                    "Confirm delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    if (auth.DeleteStaffByPhoneNumber(selectedStaff.PhoneNumber))
                    {
                        // Remove from the grid’s source
                        var staffList = (List<Staff>)staffGrid.ItemsSource;
                        staffList.Remove(selectedStaff);
                        staffGrid.Items.Refresh();

                        MessageBox.Show("Staff deleted successfully.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a staff member first.");
            }
        }

        public void resetBtn_Clcik(object sender, RoutedEventArgs e) 
        {
            AuthService authService = new AuthService();
            authService.RefreshVehicleTypeFeesFromDb();
            LoadFeeDataGrid();
        }
    }
}

