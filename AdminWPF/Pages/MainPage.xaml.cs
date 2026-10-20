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
        tabs.Items.Add(new TabItem { Header = "Квартиры", Content = new ApartmentsListView() });
        tabs.Items.Add(new TabItem { Header = "Клиенты", Content = new CrudView<ClientDto, CreateClientDto>("api/clients") });

        var catalogs = new TabControl();
        catalogs.Items.Add(new TabItem { Header = "Удобства", Content = new CrudView<AmenityDto, CreateAmenityDto>("api/amenities") });
        catalogs.Items.Add(new TabItem { Header = "Правила", Content = new CrudView<RuleDto, CreateRuleDto>("api/apartments/rules") });
        catalogs.Items.Add(new TabItem { Header = "Рядом", Content = new CrudView<NearbyPlaceDto, CreateNearbyDto>("api/apartments/nearby-places") });
        catalogs.Items.Add(new TabItem { Header = "Цены", Content = new CrudView<PriceRuleDto, CreatePriceRuleDto>("api/admin/price-rules") });
        tabs.Items.Add(new TabItem { Header = "Справочники", Content = catalogs });

        tabs.Items.Add(new TabItem { Header = "Вики", Content = new CrudView<WikiPlaceDto, CreateWikiPlaceDto>("api/wiki-places") });

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
            ApiService.ClearToken();
            NavigationService?.Navigate(new Auth());
        };

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