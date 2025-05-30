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
    /// Interaction logic for payment_form.xaml
    /// </summary>
    public partial class payment_form : Window
    {
        public payment_form()
        {
            InitializeComponent();
            LoadExitedCars();
        }



        private void LoadExitedCars()
        {
            List<Car> allCars = AuthService.GetCars();

            // Filter to get only exited cars
            //var exitedCars = allCars.Where(car => !string.IsNullOrEmpty(car.ExitTime)).ToList();

            // Bind to DataGrid
            exitedCarsGrid.ItemsSource = allCars;
        }

    }
}
