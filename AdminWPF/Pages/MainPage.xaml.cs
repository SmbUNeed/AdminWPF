using AdminWPF.Models;
using AdminWPF.Services;
using AdminWPF.Shared;
using AdminWPF.Views;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminWPF.Pages;

public partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();

        var tabs = new TabControl();
        tabs.Items.Add(new TabItem { Header = "Квартиры", Content = new ApartmentsListView() , ToolTip = "Список квартир"});

        var bookings = new TabControl();
        bookings.Items.Add(new TabItem { Header = "Брони", Content = new CrudView<AdminBookingDto, CreateBookingDto>("api/bookings", "Брони"), ToolTip = "Внутренние бронирования" });
        bookings.Items.Add(new TabItem { Header = "Внешние", Content = new CrudView<ExternalBookingDto, NoCreateDto>("api/admin/external-bookings", "Внешние брони"), ToolTip = "Внешние бронирования" });
        tabs.Items.Add(new TabItem { Header = "Бронирования", Content = bookings , ToolTip="Список бронирований"});

        tabs.Items.Add(new TabItem { Header = "Клиенты", Content = new CrudView<ClientDto, CreateClientDto>("api/clients"), ToolTip="Список клиентов" });

        var catalogs = new TabControl();
        catalogs.Items.Add(new TabItem { Header = "Удобства", Content = new CrudView<AmenityDto, CreateAmenityDto>("api/amenities"), ToolTip = "Список удобств" });
        catalogs.Items.Add(new TabItem { Header = "Правила", Content = new CrudView<RuleDto, CreateRuleDto>("api/apartments/rules") , ToolTip = "Список правил" });
        catalogs.Items.Add(new TabItem { Header = "Рядом", Content = new CrudView<NearbyPlaceDto, CreateNearbyDto>("api/apartments/nearby-places"), ToolTip = "Список мест рядом" });
        catalogs.Items.Add(new TabItem { Header = "Цены", Content = new CrudView<PriceRuleDto, CreatePriceRuleDto>("api/admin/price-rules"), ToolTip = "Список ценновых правил"  });

        tabs.Items.Add(new TabItem { Header = "Справочники", Content = catalogs, ToolTip = "Справочники" });

        tabs.Items.Add(new TabItem { Header = "Вики", Content = new CrudView<WikiPlaceDto, CreateWikiPlaceDto>("api/wiki-places"), ToolTip = "Вики" });

        tabs.Items.Add(new TabItem { Header = "Настройки", Content = new SettingsView() , ToolTip = "Настройки" });

        var passwordButton = new Button { Content = "Сменить пароль" };
        passwordButton.Click += async (_, _) =>
        {
            passwordButton.IsEnabled = false;
            var (success, error) = await ApiService.RequestPasswordChangeAsync();
            MessageBox.Show(success ? "Письмо отправлено на почту" : error ?? "Ошибка");
            passwordButton.IsEnabled = true;
        };

        var logoutButton = new Button { Content = "Выйти" };
        logoutButton.Click += (_, _) =>
        {
            if (MessageBox.Show("Выйти из аккаунта?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            ApiService.ClearToken();
            NavigationService?.Navigate(new Auth());
        };

        var ghost = (Style)Application.Current.FindResource("GhostButton");
        passwordButton.Style = ghost;
        logoutButton.Style = ghost;
        tabs.Margin = new Thickness(16, 0, 16, 16);

        var topBar = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(8) };
        topBar.Children.Add(passwordButton);
        topBar.Children.Add(logoutButton);

        var root = new DockPanel();
        DockPanel.SetDock(topBar, Dock.Top);
        root.Children.Add(topBar);
        root.Children.Add(tabs);
        Content = root;
    }
}