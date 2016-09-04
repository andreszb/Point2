using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Point.Views
{
    public sealed partial class LoginDialog : UserControl
    {
        string[] passwords = { "GRM1100456" };

        public LoginDialog()
        {
            this.InitializeComponent();

        }

        public event EventHandler HideRequested;
        public event EventHandler LoggedIn;

        private void LoginClicked(object sender, RoutedEventArgs e)
        {
            if (Array.Exists(passwords, pass => pass.Equals(PasswordTextBox.Password)))
            {
                LoggedIn?.Invoke(this, EventArgs.Empty);
                PasswordTextBox.Password = String.Empty;
            } else
            {
                PasswordTextBox.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            HideRequested?.Invoke(this, EventArgs.Empty);
        }

        private void PasswordTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginClicked(this, null);
            }
        }
    }
}
