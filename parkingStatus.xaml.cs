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
    /// Interaction logic for parkingStatus.xaml
    /// </summary>
    public partial class parkingStatus : Window
    {
        private List<ParkingSpot> spots = new List<ParkingSpot>();

        public parkingStatus()
        {
            InitializeComponent();
            InitializeSpots();
            Console.WriteLine("int spots");
        }

        private void InitializeSpots()
        {
            spots.Clear();

            for (int i = 0; i < 30; i++)
            {
                spots.Add(new ParkingSpot
                {
                    Id = i,
                    IsOccupied = (false) 
                });
            }

            RenderSpots();
        }


        private void RenderSpots()
        {
            spotGrid.Children.Clear();

            foreach (var spot in spots)
            {
                var btn = new Button
                {
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(5),
                    Background = spot.IsOccupied ? Brushes.Red : Brushes.Green,
                    Tag = spot.Id,
                    Content = $"P{spot.Id}"
                };

                btn.Click += Spot_Click;

                spotGrid.Children.Add(btn);
            }
        }

        private void Spot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                var spot = spots.FirstOrDefault(s => s.Id == id);
                if (spot != null)
                {
                    spot.IsOccupied = !spot.IsOccupied;
                    RenderSpots(); // refresh
                }
            }
        }


    }
}
