using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AdminWPF.Services;

public static class TitleBar
{
    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    public static void Apply(Window window)
    {
        Set(window);
        void Handler() => window.Dispatcher.Invoke(() => Set(window));
        ThemeManager.Changed += Handler;
        window.Closed += (_, _) => ThemeManager.Changed -= Handler;
    }

    static void Set(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero) return;

        var dark = ThemeManager.Current == "Dark";
        int mode = dark ? 1 : 0;
        int caption = dark ? 0x00161C22 : 0x00FFFFFF;
        int text = dark ? 0x00E1EBF2 : 0x0018212A;

        DwmSetWindowAttribute(hwnd, 20, ref mode, 4);
        DwmSetWindowAttribute(hwnd, 35, ref caption, 4);
        DwmSetWindowAttribute(hwnd, 36, ref text, 4);
    }
}