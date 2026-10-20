using AdminWPF.Services;
using AdminWPF.Views;
using AdminWPF;
using AdminWPF.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminWPF;

public partial class Shell : Window
{
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        TitleBar.Apply(this);
    }
    public Shell()
    {
        InitializeComponent();
        Loaded += async (_, _) => await StartAsync();
    }

    async Task StartAsync()
    {
        MainFrame.Navigate(new Auth());
    }
    private void ThemeButton_Click(object sender, RoutedEventArgs e) => ThemeManager.Toggle();
}