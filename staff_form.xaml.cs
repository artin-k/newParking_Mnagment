using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for staff_form.xaml
    /// </summary>
    public partial class staff_form : Window
    {
        private ParkingSpot? freeSpot;

        string plate = string.Empty;
        string vehicleType = string.Empty;

        public staff_form()
        {
            InitializeComponent();
        }

        public void staff_form_Load(object sender, EventArgs e)
        {
            txtEnterTime.Text = DateTime.Now.ToString("HH:mm:ss");
            AssignSpot();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (radioMotorbike.IsChecked == true)
            {
                // Show motorbike-specific input (like txtMotorPlate)
                normalPlate.Visibility = Visibility.Hidden;
                txtMotorPlate.Visibility = Visibility.Visible;
            }
            else if (radioCar.IsChecked == true)
            {
                // Show car-specific plate parts
                normalPlate.Visibility = Visibility.Visible;
                txtMotorPlate.Visibility = Visibility.Hidden;
            }
        }

        public static ParkingSpot? GetFirstFreeSpot()
        {           
            var spots = AuthService.GetAllSpots();
            return spots.FirstOrDefault(s => !s.IsOccupied);
        }

        private ParkingSpot? AssignSpot()
        {
            freeSpot = AuthService.GetFirstEmptySpot();
            if (freeSpot == null)
            {
                MessageBox.Show("No available parking spots.");
                return null;
            }
            txtParkPlace.Text = freeSpot.Id.ToString();
            return freeSpot;
        }

        public bool IsAllDigit(string str)
        {
            return str.All(char.IsDigit);
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && IsAllDigit(phoneNumber) && phoneNumber.Length == 11;
        }

        public void CarRegister_Click(object sender, EventArgs e)
        {
            if (radioMotorbike.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(txtMotorPlate.Text) || !IsAllDigit(txtMotorPlate.Text))
                {
                    MessageBox.Show("Invalid motorbike plate. Please enter only digits.");
                    return;
                }
                plate = txtMotorPlate.Text;
            }
            else // Car
            {
                if (!IsAllDigit(txtPlatePart1.Text) || txtPlatePart1.Text.Length != 2 ||
                    string.IsNullOrWhiteSpace(cmbPlateLetter.Text) ||
                    !IsAllDigit(txtPlatePart2.Text) || txtPlatePart2.Text.Length != 3)
                {
                    MessageBox.Show("Invalid car plate. Format should be like: 12 ب 345");
                    return;
                }

                plate = $"{txtPlatePart1.Text} {cmbPlateLetter.Text} {txtPlatePart2.Text}";

            }

            string phone = txtPhone.Text;

            var freeSpot = AssignSpot();
            if (freeSpot == null)
                return;



            if (txtEnterDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a valid date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCarSpecification.Text) ||
                !IsPhoneNumberValid(phone) ||
                string.IsNullOrWhiteSpace(txtParkPlace.Text) || string.IsNullOrEmpty(plate) ||
                string.IsNullOrWhiteSpace(txtEnterTime.Text))
            {
                MessageBox.Show("Bro, don't pass nulls or wrong info. Fill all the fields properly.");
                return;
            }

            string enterDateTime = txtEnterDate.SelectedDate.Value.ToString();
            string vehicleType = radioCar.IsChecked == true ? "Car" : "Motorbike";

            Car car = new Car(
                plate,
                txtCarSpecification.Text,
                txtPhone.Text,
                vehicleType,
                txtParkPlace.Text,
                txtEnterTime.Text,
                enterDateTime,
                false
            );

            AuthService authService = new AuthService();
            bool success = authService.registerCar(car);

            if (success)
            {
                AuthService.UpdateSpotStatus(freeSpot.Id, true);

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is parkingStatus psWindow)
                        psWindow.LoadSpotsFromDb();
                }

                MessageBox.Show("Register successfully");
                AssignSpot();
                CancellFunc();
            }
            else
            {
                MessageBox.Show("Register failed, try again!");
            }
        }

        public void CancellFunc()
        {
            txtPlatePart1.Text = null;
            cmbPlateLetter.SelectedIndex = 0;
            txtPlatePart2.Text = null;
            txtCarSpecification.Text = null;
            txtPhone.Text = null;
            Keyboard.ClearFocus();
        }

        public void CanselBtn_Click(object sender, EventArgs e)
        {
            CancellFunc();
        }

        public void PaymentFrmBtn_Click(object sender, EventArgs e)
        {
            var pf = new payment_form();
            pf.Show();
        }

        public void statusBtn_Click(object sender, EventArgs e)
        {
            var ps = new parkingStatus();
            ps.Show();
        }

    }
}
