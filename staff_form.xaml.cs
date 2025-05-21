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
            MessageBox.Show("load");
        }
        
        public void CarRegister_Click(object sender, EventArgs e)
        {

        }

    }
}
