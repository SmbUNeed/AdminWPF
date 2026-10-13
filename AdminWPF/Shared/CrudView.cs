using System.Windows;
using System.Windows.Controls;
using AdminWPF.Services;

namespace AdminWPF.Shared;

public class CrudView<TRead, TCreate> : UserControl where TCreate : class, new()
{
    readonly string _endpoint;
    readonly DataGrid _grid = new() { AutoGenerateColumns = true };

    public CrudView(string endpoint)
    {
        _endpoint = endpoint;
        var add = new Button { Content = "Добавить" };
        var del = new Button { Content = "Удалить" };
        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 8) };
        toolbar.Children.Add(add);
        toolbar.Children.Add(del);

        var panel = new DockPanel();
        DockPanel.SetDock(toolbar, Dock.Top);
        panel.Children.Add(toolbar);
        panel.Children.Add(_grid);
        Content = panel;

        add.Click += async (_, _) => await AddAsync();
        del.Click += async (_, _) => await DeleteAsync();
        Loaded += async (_, _) => await ReloadAsync();
    }

    async Task ReloadAsync() => _grid.ItemsSource = await ApiService.GetListAsync<TRead>(_endpoint);

    async Task AddAsync()
    {
        var dto = new TCreate();
        if (EditForm.Show(dto))
        {
            await ApiService.PostAsync(_endpoint, dto);
            await ReloadAsync();
        }
    }

    async Task DeleteAsync()
    {
        if (_grid.SelectedItem == null) return;
        var id = typeof(TRead).GetProperty("Id")?.GetValue(_grid.SelectedItem);
        await ApiService.DeleteAsync($"{_endpoint}/{id}");
        await ReloadAsync();
    }
}