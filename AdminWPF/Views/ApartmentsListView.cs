using AdminWPF.Models;
using AdminWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AdminWPF.Views
{
    public class ApartmentsListView : UserControl
    {
        readonly DataGrid _grid = new() { AutoGenerateColumns = true };
        readonly ContentControl _detail = new();

        public ApartmentsListView()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            Grid.SetColumn(_grid, 0);
            Grid.SetColumn(_detail, 1);
            grid.Children.Add(_grid);
            grid.Children.Add(_detail);
            Content = grid;

            _grid.SelectionChanged += async (_, _) => await OpenSelectedAsync();
            Loaded += async (_, _) => _grid.ItemsSource = await ApiService.GetListAsync<ApartmentSummaryAdminDto>("api/apartments/admin-summary");
        }

        async Task OpenSelectedAsync()
        {
            if (_grid.SelectedItem is not ApartmentSummaryAdminDto item) return;
            var full = await ApiService.GetAsync<ApartmentDto>($"api/apartments/admin/{item.Id}");
            if (full != null) _detail.Content = new ApartmentDetailView(full);
        }
    }
}
