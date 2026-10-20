using AdminWPF.Services;
using AdminWPF.Shared;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AdminWPF.Shared;

public class CrudView<TRead, TCreate> : UserControl where TRead : class where TCreate : class, new()
{
    const string All = "Все";
    static readonly PropertyInfo[] Props = typeof(TRead).GetProperties().Where(p => EditForm.IsSimple(p.PropertyType)).ToArray();

    readonly string _endpoint;
    readonly string _title;
    readonly DataGrid _grid = new() { AutoGenerateColumns = true };
    readonly TextBox _search = new() { Width = 200, Height = 34, Margin = new Thickness(0, 0, 12, 0) };
    readonly ComboBox _column = new() { Width = 150, Height = 34, Margin = new Thickness(0, 0, 8, 0) };
    readonly ComboBox _value = new() { Width = 170, Height = 34, Margin = new Thickness(0, 0, 12, 0) };
    List<TRead> _items = [];
    ICollectionView? _view;

    public CrudView(string endpoint, string? title = null)
    {
        _endpoint = endpoint;
        _title = title ?? typeof(TRead).Name.Replace("Dto", "");

        _column.ItemsSource = new[] { All }.Concat(Props.Select(p => p.Name)).ToList();
        _column.SelectedIndex = 0;
        _value.ItemsSource = new[] { All };
        _value.SelectedIndex = 0;

        var toolbar = new WrapPanel { Margin = new Thickness(0, 0, 0, 10) };
        toolbar.Children.Add(Label("Поиск"));
        toolbar.Children.Add(_search);
        toolbar.Children.Add(Label("Фильтр"));
        toolbar.Children.Add(_column);
        toolbar.Children.Add(_value);
        toolbar.Children.Add(Btn("Добавить", false, async () => await AddAsync()));
        toolbar.Children.Add(Btn("Изменить", true, async () => await EditAsync()));
        toolbar.Children.Add(Btn("Удалить", true, async () => await DeleteAsync()));
        toolbar.Children.Add(Btn("График", true, () => new ChartWindow(_title, GetTable()).ShowDialog()));
        toolbar.Children.Add(Btn("DOCX", true, () => Exporter.Save("docx", GetTable(), _title)));
        toolbar.Children.Add(Btn("PDF", true, () => Exporter.Save("pdf", GetTable(), _title)));
        toolbar.Children.Add(Btn("XLSX", true, () => Exporter.Save("xlsx", GetTable(), _title)));

        var panel = new DockPanel();
        DockPanel.SetDock(toolbar, Dock.Top);
        panel.Children.Add(toolbar);
        panel.Children.Add(_grid);
        Content = panel;

        _search.TextChanged += (_, _) => _view?.Refresh();
        _column.SelectionChanged += (_, _) => UpdateValues();
        _value.SelectionChanged += (_, _) => _view?.Refresh();
        Loaded += async (_, _) => await ReloadAsync();
    }

    static TextBlock Label(string text)
    {
        var label = new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 6, 0) };
        label.SetResourceReference(TextBlock.ForegroundProperty, "MutedBrush");
        return label;
    }

    static Button Btn(string text, bool ghost, Action action)
    {
        var button = new Button { Content = text };
        if (ghost) button.Style = (Style)Application.Current.FindResource("GhostButton");
        button.Click += (_, _) => action();
        return button;
    }

    static void Check(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.ServiceUnavailable) return;
        MessageBox.Show($"Ошибка {(int)response.StatusCode}", "Ошибка");
    }

    bool Match(object item)
    {
        var query = _search.Text.Trim();
        if (query.Length > 0 && !Props.Any(p => (p.GetValue(item)?.ToString() ?? "").Contains(query, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (_column.SelectedItem is string name && name != All && _value.SelectedItem is string value && value != All)
            return (Props.First(p => p.Name == name).GetValue(item)?.ToString() ?? "") == value;

        return true;
    }

    void UpdateValues()
    {
        var previous = _value.SelectedItem as string;
        var values = new List<string> { All };
        if (_column.SelectedItem is string name && name != All)
        {
            var prop = Props.First(p => p.Name == name);
            values.AddRange(_items.Select(i => prop.GetValue(i)?.ToString() ?? "").Distinct().OrderBy(v => v));
        }
        _value.ItemsSource = values;
        _value.SelectedItem = previous != null && values.Contains(previous) ? previous : All;
    }

    TableData GetTable() => new(
        Props.Select(p => p.Name).ToArray(),
        _grid.Items.Cast<object>().Select(o => Props.Select(p => p.GetValue(o)?.ToString() ?? "").ToArray()).ToList());

    async Task ReloadAsync()
    {
        _items = await ApiService.GetListAsync<TRead>(_endpoint) ?? [];
        _grid.ItemsSource = _items;
        _view = CollectionViewSource.GetDefaultView(_items);
        _view.Filter = Match;
        UpdateValues();
    }

    async Task AddAsync()
    {
        var dto = new TCreate();
        if (!EditForm.Show(dto)) return;
        Check(await ApiService.PostAsync(_endpoint, dto));
        await ReloadAsync();
    }

    async Task EditAsync()
    {
        if (_grid.SelectedItem is not TRead item) return;
        if (!EditForm.Show(item)) return;
        var id = typeof(TRead).GetProperty("Id")?.GetValue(item);
        Check(await ApiService.PutAsync($"{_endpoint}/{id}", item));
        await ReloadAsync();
    }

    async Task DeleteAsync()
    {
        if (_grid.SelectedItem is not TRead item) return;
        if (MessageBox.Show("Удалить запись?", "Подтверждение", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        var id = typeof(TRead).GetProperty("Id")?.GetValue(item);
        Check(await ApiService.DeleteAsync($"{_endpoint}/{id}"));
        await ReloadAsync();
    }
}