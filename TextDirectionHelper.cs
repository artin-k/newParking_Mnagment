using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public static class TextDirectionHelper
{
    public static void ApplyToTextBox(TextBox textBox)
    {
        textBox.TextChanged += (sender, e) =>
        {
            var text = textBox.Text;
            textBox.FlowDirection = GetFlowDirection(text);
        };
    }

    public static void ApplyToPasswordBox(PasswordBox passwordBox)
    {
        passwordBox.PasswordChanged += (sender, e) =>
        {
            var text = passwordBox.Password;
            passwordBox.FlowDirection = GetFlowDirection(text);
        };
    }

    private static FlowDirection GetFlowDirection(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            char firstChar = text[0];
            if ((firstChar >= 0x0600 && firstChar <= 0x06FF) ||    // Arabic / Persian
                (firstChar >= 0x0750 && firstChar <= 0x077F) ||
                (firstChar >= 0xFB50 && firstChar <= 0xFDFF))
            {
                return FlowDirection.RightToLeft;
            }
        }

        return FlowDirection.LeftToRight;
    }
}
