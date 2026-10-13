using AdminWPF.Services;
using AdminWPF.Pages;
using System.Windows;
using System.Windows.Controls;

namespace AdminWPF.Views;

public partial class Auth : Page
{
    public Auth()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
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

        TokenStorage.Save(auth.Token);
        ApiService.SetToken(auth.Token);
        NavigationService?.Navigate(new MainPage());
    }
}