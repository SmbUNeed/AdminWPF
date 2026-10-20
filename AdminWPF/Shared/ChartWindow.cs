using System.Windows;
using System.Windows.Controls;
using AdminWPF.Services;

namespace AdminWPF.Shared;

public class ChartWindow : Window
{
    record Metric(string Name, int Column, string Kind);

    readonly StackPanel _bars = new() { Margin = new Thickness(16, 0, 16, 16) };
    readonly ComboBox _category = new() { Width = 200, Height = 34, Margin = new Thickness(0, 0, 12, 0) };
    readonly ComboBox _metric = new() { Width = 260, Height = 34, DisplayMemberPath = nameof(Metric.Name) };
    readonly TableData _table;

    public ChartWindow(string title, TableData table)
    {
        _table = table;
        Title = title;
        Width = 800;
        Height = 520;
        Owner = Application.Current.MainWindow;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        SetResourceReference(BackgroundProperty, "BackgroundBrush");
        SetResourceReference(ForegroundProperty, "ForegroundBrush");
        SourceInitialized += (_, _) => TitleBar.Apply(this);

        _category.ItemsSource = table.Columns;
        _category.SelectedIndex = table.Columns.Length == 0 ? -1 : DefaultCategory();

        var metrics = new List<Metric> { new("Количество записей", -1, "count") };
        for (var c = 0; c < table.Columns.Length; c++)
        {
            if (!IsNumeric(c)) continue;
            metrics.Add(new Metric($"Сумма: {table.Columns[c]}", c, "sum"));
            metrics.Add(new Metric($"Среднее: {table.Columns[c]}", c, "avg"));
        }
        _metric.ItemsSource = metrics;
        _metric.SelectedIndex = 0;

        _category.SelectionChanged += (_, _) => Draw();
        _metric.SelectionChanged += (_, _) => Draw();

        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(16) };
        toolbar.Children.Add(new TextBlock { Text = "Группировать по", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 8, 0) });
        toolbar.Children.Add(_category);
        toolbar.Children.Add(new TextBlock { Text = "Значение", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 8, 0) });
        toolbar.Children.Add(_metric);

        var panel = new DockPanel();
        DockPanel.SetDock(toolbar, Dock.Top);
        panel.Children.Add(toolbar);
        panel.Children.Add(new ScrollViewer { Content = _bars });
        Content = panel;

        Draw();
    }

    bool IsNumeric(int column)
    {
        if (_table.Columns[column].EndsWith("Id")) return false;
        var values = _table.Rows.Select(r => r[column]).Where(v => v.Length > 0).ToList();
        return values.Count > 0 && values.All(v => double.TryParse(v, out _));
    }

    int DefaultCategory()
    {
        for (var c = 0; c < _table.Columns.Length; c++)
        {
            if (_table.Columns[c].EndsWith("Id")) continue;
            var distinct = _table.Rows.Select(r => r[c]).Distinct().Count();
            if (distinct > 1 && distinct < _table.Rows.Count) return c;
        }
        return 0;
    }

    void Draw()
    {
        _bars.Children.Clear();
        if (_category.SelectedIndex < 0 || _metric.SelectedItem is not Metric metric) return;
        var column = _category.SelectedIndex;

        var groups = _table.Rows
            .GroupBy(r => string.IsNullOrEmpty(r[column]) ? "—" : r[column])
            .Select(g => (Key: g.Key, Value: Calculate(g, metric)))
            .OrderByDescending(g => g.Value)
            .Take(20)
            .ToList();

        var max = groups.Count == 0 ? 1 : Math.Max(groups.Max(g => g.Value), 0.0001);
        foreach (var (key, value) in groups)
            _bars.Children.Add(BuildRow(key, value, max));
    }

    static double Calculate(IEnumerable<string[]> rows, Metric metric)
    {
        var list = rows.ToList();
        if (metric.Kind == "count") return list.Count;

        var numbers = list.Select(r => double.TryParse(r[metric.Column], out var n) ? n : 0).ToList();
        return metric.Kind == "sum" ? numbers.Sum() : numbers.Average();
    }

    static Grid BuildRow(string key, double value, double max)
    {
        var row = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90) });

        var label = new TextBlock
        {
            Text = key,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 12, 0)
        };

        var shown = Math.Max(value, 0);
        var track = new Grid();
        track.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(shown, GridUnitType.Star) });
        track.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Math.Max(max - shown, 0), GridUnitType.Star) });
        var bar = new Border { Height = 22, CornerRadius = new CornerRadius(6) };
        bar.SetResourceReference(Border.BackgroundProperty, "AccentBrush");
        track.Children.Add(bar);

        var number = new TextBlock
        {
            Text = value.ToString("0.##"),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(8, 0, 0, 0)
        };

        Grid.SetColumn(track, 1);
        Grid.SetColumn(number, 2);
        row.Children.Add(label);
        row.Children.Add(track);
        row.Children.Add(number);
        return row;
    }
}