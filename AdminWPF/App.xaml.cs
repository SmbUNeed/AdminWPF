using AdminWPF.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace AdminWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeManager.Load();
            base.OnStartup(e);
        }
        public App()
        {
            DispatcherUnhandledException += (_, e) =>
            {
                MessageBox.Show(e.Exception.Message, "Ошибка");
                e.Handled = true;
            };
        }
    }

}
