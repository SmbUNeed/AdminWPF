using AdminWPF.Models;
using AdminWPF.Services;
using AdminWPF.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminWPF.Views
{
    /// <summary>
    /// Логика взаимодействия для ApartmentDetailView.xaml
    /// </summary>
    public partial class ApartmentDetailView : UserControl
    {
        readonly ApartmentDto _apartment;
        MultiSelectTab<AmenityDto>? _amenitiesTab;
        MultiSelectTab<RuleDto>? _rulesTab;
        MultiSelectTab<NearbyPlaceDto>? _nearbyTab;

        public ApartmentDetailView(ApartmentDto apartment)
        {
            _apartment = apartment;
            InitializeComponent();
            Loaded += async (_, _) => await LoadAsync();
            SaveButton.Click += async (_, _) => await SaveAsync();
        }

        async Task LoadAsync()
        {
            TitleBox.Text = _apartment.Title;
            AddressBox.Text = _apartment.Address;
            DescriptionBox.Text = _apartment.Description;

            var amenities = await ApiService.GetListAsync<AmenityDto>("api/amenities") ?? [];
            _amenitiesTab = new MultiSelectTab<AmenityDto>(amenities, _apartment.Amenities.Select(a => a.Id), a => a.Id, nameof(AmenityDto.Name));
            AmenitiesTabItem.Content = _amenitiesTab;

            var rules = await ApiService.GetListAsync<RuleDto>("api/apartments/rules") ?? [];
            _rulesTab = new MultiSelectTab<RuleDto>(rules, _apartment.Rules.Select(r => r.Id), r => r.Id, nameof(RuleDto.Text));
            RulesTabItem.Content = _rulesTab;

            var nearby = await ApiService.GetListAsync<NearbyPlaceDto>("api/apartments/nearby-places") ?? [];
            _nearbyTab = new MultiSelectTab<NearbyPlaceDto>(nearby, _apartment.Nearby.Select(n => n.Id), n => n.Id, nameof(NearbyPlaceDto.Name));
            NearbyTabItem.Content = _nearbyTab;
        }

        async Task SaveAsync()
        {
            var dto = new UpdateApartmentDto(_apartment.Id, _apartment.AdminTag, TitleBox.Text, AddressBox.Text, DescriptionBox.Text,
                _apartment.Guests, _apartment.GuestOverpayment, _apartment.PricePerNight, _apartment.RoomsCount,
                _apartment.Area, _apartment.IsActive, _apartment.IsDeleted, _apartment.MainImage);

            await ApiService.PutAsync($"api/apartments/{_apartment.Id}", dto);
            await ApiService.PutAsync($"api/apartments/{_apartment.Id}/amenities", new SetIdsDto(_amenitiesTab!.GetSelectedIds()));
            await ApiService.PutAsync($"api/apartments/{_apartment.Id}/rules", new SetIdsDto(_rulesTab!.GetSelectedIds()));
            await ApiService.PutAsync($"api/apartments/{_apartment.Id}/nearby", new SetIdsDto(_nearbyTab!.GetSelectedIds()));
        }
    }
}
