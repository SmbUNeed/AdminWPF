using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace AdminWPF.Shared;

public static class EditForm
{
    public static bool Show(object dto)
    {
        var win = new Window { Width = 350, SizeToContent = SizeToContent.Height, WindowStartupLocation = WindowStartupLocation.CenterScreen, Title = "Редактирование" };
        var stack = new StackPanel { Margin = new Thickness(20) };
        var props = dto.GetType().GetProperties().Where(p => p.CanWrite).ToArray();
        var controls = new Dictionary<PropertyInfo, Control>();

        foreach (var prop in props)
        {
            stack.Children.Add(new TextBlock { Text = prop.Name, Margin = new Thickness(0, 8, 0, 2) });
            Control control = prop.PropertyType == typeof(bool)
                ? new CheckBox { IsChecked = (bool)(prop.GetValue(dto) ?? false) }
                : new TextBox { Text = prop.GetValue(dto)?.ToString() ?? "" };
            controls[prop] = control;
            stack.Children.Add(control);
        }

        var ok = new Button { Content = "Сохранить", Margin = new Thickness(0, 16, 0, 0) };
        bool result = false;
        ok.Click += (_, _) =>
        {
            foreach (var (prop, control) in controls)
            {
                var target = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                object? value = control is CheckBox cb ? cb.IsChecked ?? false : Convert.ChangeType(((TextBox)control).Text, target);
                prop.SetValue(dto, value);
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