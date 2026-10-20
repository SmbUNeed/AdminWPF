using AdminWPF.Services;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace AdminWPF.Shared;

public static class EditForm
{
    public static bool IsSimple(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal);
    }

    public static bool Show(object dto)
    {
        var win = new Window
        {
            Width = 380,
            SizeToContent = SizeToContent.Height,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow,
            Title = "Редактирование"
        };
        win.SetResourceReference(Control.BackgroundProperty, "BackgroundBrush");
        win.SetResourceReference(Control.ForegroundProperty, "ForegroundBrush");
        win.SourceInitialized += (_, _) => TitleBar.Apply(win);

        var stack = new StackPanel { Margin = new Thickness(20) };
        var controls = new Dictionary<PropertyInfo, Control>();

        foreach (var prop in dto.GetType().GetProperties().Where(p => p.CanWrite && IsSimple(p.PropertyType)))
        {
            stack.Children.Add(new TextBlock { Text = prop.Name, Margin = new Thickness(0, 8, 0, 2) });
            Control control = prop.PropertyType == typeof(bool)
                ? new CheckBox { IsChecked = (bool)(prop.GetValue(dto) ?? false) }
                : new TextBox { Text = prop.GetValue(dto)?.ToString() ?? "", IsEnabled = prop.Name != "Id" };
            controls[prop] = control;
            stack.Children.Add(control);
        }

        var ok = new Button { Content = "Сохранить", Margin = new Thickness(0, 16, 0, 0) };
        var result = false;
        ok.Click += (_, _) =>
        {
            if (controls.Where(c => c.Key.Name != "Id").All(c => c.Value is TextBox tb && string.IsNullOrWhiteSpace(tb.Text) || c.Value is CheckBox { IsChecked: not true }))
            {
                MessageBox.Show("Заполни хотя бы одно поле", "Внимание");
                return;
            }
            try
            {
                foreach (var (prop, control) in controls)
                {
                    if (prop.Name == "Id") continue;
                    var nullable = Nullable.GetUnderlyingType(prop.PropertyType);
                    var target = nullable ?? prop.PropertyType;
                    object? value = control is CheckBox cb
                        ? cb.IsChecked ?? false
                        : string.IsNullOrWhiteSpace(((TextBox)control).Text) && nullable != null
                            ? null
                            : Convert.ChangeType(((TextBox)control).Text, target);
                    prop.SetValue(dto, value);
                }
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
            {
                MessageBox.Show("Проверь формат полей", "Ошибка");
                return;
            }
            result = true;
            win.Close();
        };
        stack.Children.Add(ok);
        win.Content = stack;
        win.ShowDialog();
        return result;
    }
}