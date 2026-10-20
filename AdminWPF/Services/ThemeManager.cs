using System.IO;
using System.Windows;

namespace AdminWPF.Services;

public static class ThemeManager
{
    static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AdminMV", "theme.txt");

    public static string Current { get; private set; } = "Dark";

    public static event Action? Changed;

    public static void Load()
    {
        var saved = File.Exists(FilePath) ? File.ReadAllText(FilePath).Trim() : "Dark";
        Apply(saved == "Light" ? "Light" : "Dark");
    }

    public static void Toggle() => Apply(Current == "Dark" ? "Light" : "Dark");

    static void Apply(string name)
    {
        var dicts = Application.Current.Resources.MergedDictionaries;
        var old = dicts.FirstOrDefault(d => d.Source?.OriginalString.EndsWith("Theme.xaml") == true);
        if (old != null) dicts.Remove(old);
        dicts.Insert(0, new ResourceDictionary { Source = new Uri($"Themes/{name}Theme.xaml", UriKind.Relative) });
        Current = name;

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, name);
        }
        catch (IOException) { }

        Changed?.Invoke();
    }
}