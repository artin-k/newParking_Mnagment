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

        private void ExistedCarBtn_Click(object sender, RoutedEventArgs e)
        {

            if (exitedCarsGrid.Visibility == Visibility.Hidden)
                exitedCarsGrid.Visibility = Visibility.Visible;
            else
                exitedCarsGrid.Visibility = Visibility.Hidden;
        }
        
        private void paymentBtn_Click( object sender, RoutedEventArgs e)
        {
            if (payStack.Visibility == Visibility.Hidden)
                payStack.Visibility = Visibility.Visible;
            else
                payStack.Visibility = Visibility.Hidden;
        }

        private void CalcPayment_click(object sender, RoutedEventArgs e) 
        {
            string plate = $"{txtPlatePart1.Text} {cmbPlateLetter.Text} {txtPlatePart2.Text}";
            double amount = AuthService.ExitCar(plate);
            lblPayment.Content = $"Payment: {amount} currency units";
        }

    }
}
