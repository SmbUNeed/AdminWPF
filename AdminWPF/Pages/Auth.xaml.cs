using AdminWPF.Services;
using AdminWPF.Pages;
using System.Windows;
using System.Windows.Controls;
using AdminWPF.Windows;

namespace AdminWPF.Views;

public partial class Auth : Page
{
    int attempts = 0;
    public Auth()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (++attempts >= 3) {
            var win = new Captcha();
            if (win.ShowDialog() != true) return;
            else attempts = 0;
        }
        LoginButton.IsEnabled = false;
        ErrorText.Visibility = Visibility.Collapsed;

        var (success, auth, error) = await ApiService.LoginAsync(EmailBox.Text, PasswordBox.Password);

        if (!success || auth?.Token == null)
        {
            ErrorText.Text = error ?? "Ошибка входа";
            ErrorText.Visibility = Visibility.Visible;
            LoginButton.IsEnabled = true;
            return;
        }

        ApiService.SetToken(auth.Token);
        NavigationService?.Navigate(new MainPage());
    }
}