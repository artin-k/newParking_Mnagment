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

            carDataGrid.ItemsSource = allCars;
        }

        private void ExistedCarBtn_Click(object sender, RoutedEventArgs e)
        {

            if (carDataGrid.Visibility == Visibility.Hidden)
                carDataGrid.Visibility = Visibility.Visible;
            else
                carDataGrid.Visibility = Visibility.Hidden;
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

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            AuthService auth = new AuthService();

            if (carDataGrid.SelectedItem is Car selectedCar)
            {
                var confirm = MessageBox.Show(
                    $"Delete car with plate {selectedCar.Plate}?",
                    "Confirm delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    if (auth.DeleteCar(selectedCar.Plate))
                    {
                        // Remove from the grid’s source
                        var cars = (List<Car>)carDataGrid.ItemsSource;
                        cars.Remove(selectedCar);
                        carDataGrid.Items.Refresh();

                        MessageBox.Show("Car deleted successfully.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a car first.");
            }
        }


        private void Update_Click(object sender, RoutedEventArgs e)
        {
            var cars = (List<Car>)carDataGrid.ItemsSource;
            AuthService authService = new AuthService();    
            int successCount = 0;

            foreach (var car in cars)
            {
                if (authService.UpdateCar(car)) // Replace with actual service class
                {
                    successCount++;
                }
            }

            MessageBox.Show($"{successCount} cars updated successfully.");
        }



    }
}
