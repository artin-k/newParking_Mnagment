using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfTest
{
    public partial class parkingStatus : Window
    {
        private List<ParkingSpot> spots = new List<ParkingSpot>();

        public parkingStatus()
        {
            InitializeComponent();
            LoadSpotsFromDb();
        }

        public void LoadSpotsFromDb()
        {
            var spots = AuthService.GetAllSpots();
            this.spots = spots;
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
                    Margin = new Thickness(1),
                    Background = (Brush)new BrushConverter().ConvertFromString(spot.IsOccupied ? "#e74c3c" : "#28b463"),
                    Tag = spot.Id,
                    Content = $"P{spot.Id}"
                };


                // Apply the style
                btn.Style = (Style)FindResource("SpotButtonStyle");

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
                    // Toggle the state
                    spot.IsOccupied = !spot.IsOccupied;

                    // Update DB
                    AuthService.UpdateSpotStatus(spot.Id, spot.IsOccupied);

                    // Re-render UI
                    RenderSpots();
                }
            }
        }
    }
}
