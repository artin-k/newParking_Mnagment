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
    }
}
