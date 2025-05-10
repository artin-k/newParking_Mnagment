using System.Windows.Controls;
using System.Windows;

namespace WpfTest
{
    public partial class PlaceholderTextBox : UserControl
    {
        public PlaceholderTextBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(""));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Placeholder.Visibility = Visibility.Collapsed;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Placeholder.Visibility = string.IsNullOrWhiteSpace(Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }

}
