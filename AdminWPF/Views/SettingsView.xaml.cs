using AdminWPF.Models;
using AdminWPF.Services;
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

namespace AdminWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        readonly TextBox _header = new() { AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, Height = 80 };
        readonly TextBox _minDays = new();
        readonly TextBox _maxRange = new();
        readonly TextBox _overpayment = new();
        readonly TextBlock _info = new() { Margin = new Thickness(0, 8, 0, 0) };
        SiteSettingsDto _settings = new();

        public SettingsView()
        {
            var stack = new StackPanel { Margin = new Thickness(20), MaxWidth = 500, HorizontalAlignment = HorizontalAlignment.Left };
            AddField(stack, "Описание в шапке", _header);
            AddField(stack, "Мин. дней до брони", _minDays);
            AddField(stack, "Макс. длительность брони (дней)", _maxRange);
            AddField(stack, "Доплата за час по умолчанию", _overpayment);

            var save = new Button { Content = "Сохранить", Margin = new Thickness(0, 16, 0, 0), Height = 32 };
            save.Click += async (_, _) => await SaveAsync();
            stack.Children.Add(save);
            stack.Children.Add(_info);
            Content = stack;

            Loaded += async (_, _) => await LoadAsync();
        }

        static void AddField(StackPanel panel, string label, Control box)
        {
            panel.Children.Add(new TextBlock { Text = label, Margin = new Thickness(0, 8, 0, 2) });
            panel.Children.Add(box);
        }

        async Task LoadAsync()
        {
            _settings = await ApiService.GetAsync<SiteSettingsDto>("api/settings") ?? new SiteSettingsDto();
            _header.Text = _settings.HeaderDescription;
            _minDays.Text = _settings.MinBookingDaysAhead.ToString();
            _maxRange.Text = _settings.MaxBookingRangeDays.ToString();
            _overpayment.Text = _settings.DefaultHourOverpayment.ToString();
        }

        async Task SaveAsync()
        {
            if (!int.TryParse(_minDays.Text, out var min) ||
                !int.TryParse(_maxRange.Text, out var max) ||
                !double.TryParse(_overpayment.Text, out var over))
            {
                _info.Text = "Проверь числовые поля";
                return;
            }

            _settings.HeaderDescription = _header.Text;
            _settings.MinBookingDaysAhead = min;
            _settings.MaxBookingRangeDays = max;
            _settings.DefaultHourOverpayment = over;

            var response = await ApiService.PutAsync("api/admin/settings", _settings);
            _info.Text = response.IsSuccessStatusCode ? "Сохранено" : $"Ошибка {(int)response.StatusCode}";
        }
    }
}
