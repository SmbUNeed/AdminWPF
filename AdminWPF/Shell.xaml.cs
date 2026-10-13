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
    public Shell()
    {
        InitializeComponent();
        Loaded += async (_, _) => await StartAsync();
    }

    async Task StartAsync()
    {
        var token = TokenStorage.Load();
        if (token != null)
        {
            ApiService.SetToken(token);
            MainFrame.Navigate(new MainPage());
            return;
        }
        MainFrame.Navigate(new Auth());
    }
}