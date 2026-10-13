using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Models;

public record ClientDto(int Id, string FullName, string PhoneNumber, string? Email, int? UserId, string? AdminTag, string? AdminNote, bool InBlackList);
public record CreateClientDto { public string FullName = ""; public string PhoneNumber = ""; public string? Email; public string? AdminTag; public string? AdminNote; public bool InBlackList; }

public record AmenityDto(int Id, string Name, string Icon);
public class CreateAmenityDto { public string Name = ""; public string Icon = ""; }

public record RuleDto(int Id, string Icon, string Text);
public class CreateRuleDto { public string Icon = ""; public string Text = ""; }

public record NearbyPlaceDto(int Id, string Icon, string Name, string? Distance, int? WikiPlaceId);
public class CreateNearbyDto { public string Icon = ""; public string Name = ""; public int? WikiPlaceId; }

public record PriceRuleDto(int Id, int? ApartmentId, string Label, string StartDate, string? EndDate, bool IsRecurringYearly, double? Multiplier, double? FixedMarkup);
public class CreatePriceRuleDto { public string Label = ""; public string StartDate = ""; public string? EndDate; public bool IsRecurringYearly; public double? Multiplier; public double? FixedMarkup; }

public record WikiPlaceDto(int Id, string Name, string Description, string PhotoPath, string Type, int? NearbyPlaceId);
public class CreateWikiPlaceDto { public string Name = ""; public string Description = ""; public string? PhotoPath; public string Type = ""; public int? NearbyPlaceId; }

public record SiteSettingsDto(int Id, string HeaderDescription, int MinBookingDaysAhead, int MaxBookingRangeDays, double DefaultHourOverpayment);

public record ApartmentDto(int Id, string AdminTag, string Title, string Address, string Description, int Guests, double GuestOverpayment, double PricePerNight, int RoomsCount, double Area, bool IsActive, bool IsDeleted, string? MainImage, List<AmenityDto> Amenities, List<RuleDto> Rules, List<NearbyPlaceDto> Nearby);
public record UpdateApartmentDto(int Id, string AdminTag, string Title, string Address, string Description, int Guests, double GuestOverpayment, double PricePerNight, int RoomsCount, double Area, bool IsActive, bool IsDeleted, string? MainImage);
public record ApartmentSummaryAdminDto(int Id, string Title, string AdminTag, string? MainImage, double PricePerNight, bool IsDeleted, bool IsActive);

public record SetIdsDto(List<int> Ids);
