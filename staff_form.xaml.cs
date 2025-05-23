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
        public staff_form()
        {
            InitializeComponent();
        }

        public void staff_form_Load(object sender, EventArgs e)
        {
            txtEnterTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            AssignStaffToSpot();
        }

        public static ParkingSpot? GetFirstFreeSpot()
        {
            
            var spots = AuthService.GetAllSpots();
            return spots.FirstOrDefault(s => !s.IsOccupied);
        }

        private void AssignStaffToSpot()
        {
            
            var freeSpot = AuthService.GetFirstEmptySpot();

            if (freeSpot == null)
            {
                MessageBox.Show("No available parking spots.");
                return;
            }

            AuthService.UpdateSpotStatus(freeSpot.Id, true);
            txtParkPlace.Text = freeSpot.Id.ToString();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is parkingStatus psWindow)
                {
                    psWindow.LoadSpotsFromDb();
                }
            }
        }





        public void CarRegister_Click(object sender, EventArgs e)
        {

        }

    }
}
