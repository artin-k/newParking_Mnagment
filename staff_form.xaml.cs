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

        public staff_form()
        {
            InitializeComponent();
        }

        public void staff_form_Load(object sender, EventArgs e)
        {
            txtEnterTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            AssignSpot();
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

        public void CarRegister_Click(object sender, EventArgs e)
        {
            var freeSpot = AssignSpot();
            if (freeSpot == null)
                return;

            if (txtPlatePart1.Text.Length != 2 || string.IsNullOrWhiteSpace(cmbPlateLetter.Text) || txtPlatePart2.Text.Length != 3)
            {
                MessageBox.Show("Bro, don't pass nulls. Fill all the fields properly.");
                return;
            }

            string plate = $"{txtPlatePart1.Text} {cmbPlateLetter.Text} {txtPlatePart2.Text}";

            if (txtEnterDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a valid date.");
                return;
            }

            string enterDateTime = txtEnterDate.SelectedDate.Value.ToString();

            if (string.IsNullOrWhiteSpace(plate) || string.IsNullOrWhiteSpace(txtCarSpecification.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) || string.IsNullOrWhiteSpace(txtParkPlace.Text) ||
                string.IsNullOrWhiteSpace(txtEnterTime.Text) || string.IsNullOrEmpty(cmbVehicleType.Text))
            {
                MessageBox.Show("Bro, don't pass nulls. Fill all the fields properly.");
                return;
            }

            Car car = new Car(plate, txtCarSpecification.Text, txtPhone.Text, cmbVehicleType.Text, txtParkPlace.Text, txtEnterTime.Text, enterDateTime, false);

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
            }
            else
            {
                MessageBox.Show("Register failed, try again!");
            }
        }

        public void PaymentFrmBtn_Click(object sender, EventArgs e)
        {
            var pf = new payment_form();
            pf.Show();
        }



    }
}
