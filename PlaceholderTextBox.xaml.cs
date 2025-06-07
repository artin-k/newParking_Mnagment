using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfTest
{
    public partial class PlaceholderTextBox : UserControl
    {
        public PlaceholderTextBox()
        {
            InitializeComponent();

            // Ensure placeholder visibility is synced initially
            Loaded += (s, e) => UpdatePlaceholderVisibility();
        }

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(PlaceholderTextBox), new PropertyMetadata(0));

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlaceholderTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PlaceholderTextBox)d;
            control.UpdatePlaceholderVisibility();
        }

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(string.Empty));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePlaceholderVisibility();
        }

        private void TextBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = TextBoxInput.Text; // sync in case TextBox changed directly
            UpdatePlaceholderVisibility();
        }

        private void UpdatePlaceholderVisibility()
        {
            Placeholder.Visibility = string.IsNullOrWhiteSpace(TextBoxInput.Text) && !TextBoxInput.IsFocused
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}
