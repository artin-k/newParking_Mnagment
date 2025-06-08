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
    /// Interaction logic for Admin_form.xaml
    /// </summary>
    public partial class Admin_form : Window
    {
        public Admin_form()
        {
            InitializeComponent();

            LoadExitedCars();
        }

        private void LoadExitedCars()
        {
            List<Manager> allManagers = AuthService.GetManagers();

            managerDataGrid.ItemsSource = allManagers;
        }

        public void setManager_Click(object sender, RoutedEventArgs e)
        {
            if(managerPanel.Visibility == Visibility.Hidden) 
                managerPanel.Visibility = Visibility.Visible;
            else
                managerPanel.Visibility = Visibility.Hidden;
        }

        public void saveManager_click(object sender, RoutedEventArgs e)
        {
            if (txtFullName == null || txtUsername == null || txtPassword == null)
            {
                MessageBox.Show("insert the textboxes");
            }
            else 
            {
                AuthService authService = new AuthService();
                bool success =  authService.setManager(txtFullName.Text, txtUsername.Text, txtPassword.Text);
                if (success)
                    MessageBox.Show("manager saved successfully");
                else
                    MessageBox.Show("manager saving failed");
            }

        }

        public void viewManagers_Click(object sender, RoutedEventArgs e)
        {
            if (managerDataGrid.Visibility == Visibility.Hidden)
                managerDataGrid.Visibility = Visibility.Visible;
            else
                managerDataGrid.Visibility = Visibility.Hidden;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            AuthService auth = new AuthService();

            if (managerDataGrid.SelectedItem is Manager selectedMan)
            {
                var confirm = MessageBox.Show(
                    $"Delete manager with name {selectedMan.Name}?",
                    "Confirm delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    if (auth.DeleteManager(selectedMan.Name))
                    {
                        // Remove from the grid’s source
                        var managers = (List<Manager>)managerDataGrid.ItemsSource;
                        managers.Remove(selectedMan);
                        managerDataGrid.Items.Refresh();

                        MessageBox.Show("manager deleted successfully.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Select a manager first.");
            }
        }
    }
}
