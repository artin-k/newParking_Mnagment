using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

public class PlaceholderTextBox : TextBox
{
    public string PlaceholderText { get; set; } = "";
    private bool _isPlaceholderVisible = true;

    public PlaceholderTextBox()
    {
        Loaded += (s, e) => ShowPlaceholderIfEmpty();
        GotFocus += (s, e) => RemovePlaceholder();
        LostFocus += (s, e) => ShowPlaceholderIfEmpty();
        TextChanged += (s, e) =>
        {
            if (!_isPlaceholderVisible)
                FlowDirection = GetFlowDirection(Text);

            if (string.IsNullOrEmpty(Text) && !IsFocused)
            {
                ShowPlaceholderIfEmpty();
            }
        };
        Background = Brushes.Transparent;
    }

    private void ShowPlaceholderIfEmpty()
    {
        if (string.IsNullOrWhiteSpace(base.Text))
        {
            _isPlaceholderVisible = true;
            base.Text = PlaceholderText;
            Foreground = Brushes.Gray;
            Background = Brushes.Transparent;
        }
    }

    private void RemovePlaceholder()
    {
        if (_isPlaceholderVisible)
        {
            _isPlaceholderVisible = false;
            base.Text = "";
            Foreground = Brushes.Black;
            Background = Brushes.Transparent;
        }
    }

    public new string Text
    {
        get => _isPlaceholderVisible ? "" : base.Text;
        set
        {
            base.Text = value;
            _isPlaceholderVisible = string.IsNullOrWhiteSpace(value);
            Foreground = _isPlaceholderVisible ? Brushes.Gray : Brushes.Black;
            FlowDirection = GetFlowDirection(value);
            Background = Brushes.Transparent;
        }
    }

    private FlowDirection GetFlowDirection(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            char firstChar = text[0];
            if ((firstChar >= 0x0600 && firstChar <= 0x06FF) || // Arabic/Persian
                (firstChar >= 0x0750 && firstChar <= 0x077F) ||
                (firstChar >= 0xFB50 && firstChar <= 0xFDFF))
            {
                return FlowDirection.RightToLeft;
            }
        }
        return FlowDirection.LeftToRight;
    }
}
