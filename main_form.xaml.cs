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
    public partial class main_form : Window
    {
        public main_form()
        {
            InitializeComponent();
        }
        
        private void managerForm_Click(object sender, RoutedEventArgs e)
        {
            var mf = new manager_form();
            mf.Show();
        }
    }
}
