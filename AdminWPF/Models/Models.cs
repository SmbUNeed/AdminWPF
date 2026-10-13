using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF.Models;

public record ClientDto(int Id, string FullName, string PhoneNumber, string? Email, int? UserId, string? AdminTag, string? AdminNote, bool InBlackList);
public class CreateClientDto { public string FullName { get; set; } = ""; public string PhoneNumber { get; set; } = ""; public string? Email { get; set; } public string? AdminTag { get; set; } public string? AdminNote { get; set; } public bool InBlackList { get; set; } }

public record AmenityDto(int Id, string Name, string Icon);
public class CreateAmenityDto { public string Name { get; set; } = ""; public string Icon { get; set; } = ""; }

public record RuleDto(int Id, string Icon, string Text);
public class CreateRuleDto { public string Icon { get; set; } = ""; public string Text { get; set; } = ""; }

public record NearbyPlaceDto(int Id, string Icon, string Name, string? Distance, int? WikiPlaceId);
public class CreateNearbyDto { public string Icon { get; set; } = ""; public string Name { get; set; } = ""; public int? WikiPlaceId { get; set; } }

public record PriceRuleDto(int Id, int? ApartmentId, string Label, string StartDate, string? EndDate, bool IsRecurringYearly, double? Multiplier, double? FixedMarkup);
public class CreatePriceRuleDto { public string Label { get; set; } = ""; public string StartDate { get; set; } = ""; public string? EndDate { get; set; } public bool IsRecurringYearly { get; set; } public double? Multiplier { get; set; } public double? FixedMarkup { get; set; } }

public record WikiPlaceDto(int Id, string Name, string Description, string PhotoPath, string Type, int? NearbyPlaceId);
public class CreateWikiPlaceDto { public string Name { get; set; } = ""; public string Description { get; set; } = ""; public string? PhotoPath { get; set; } public string Type { get; set; } = ""; public int? NearbyPlaceId { get; set; } }

public record SiteSettingsDto(int Id, string HeaderDescription, int MinBookingDaysAhead, int MaxBookingRangeDays, double DefaultHourOverpayment);

public record ApartmentDto(int Id, string AdminTag, string Title, string Address, string Description, int Guests, double GuestOverpayment, double PricePerNight, int RoomsCount, double Area, bool IsActive, bool IsDeleted, string? MainImage, List<AmenityDto> Amenities, List<RuleDto> Rules, List<NearbyPlaceDto> Nearby);
public record UpdateApartmentDto(int Id, string AdminTag, string Title, string Address, string Description, int Guests, double GuestOverpayment, double PricePerNight, int RoomsCount, double Area, bool IsActive, bool IsDeleted, string? MainImage);
public record ApartmentSummaryAdminDto(int Id, string Title, string AdminTag, string? MainImage, double PricePerNight, bool IsDeleted, bool IsActive);

public record SetIdsDto(List<int> Ids);
