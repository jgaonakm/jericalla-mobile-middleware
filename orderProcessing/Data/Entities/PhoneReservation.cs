using System.ComponentModel.DataAnnotations;
namespace Data.Entities;

public class PhoneReservation
{
    [Key] public int Id { get; set; }
    [Required] public required string PlanType { get; set; }

    [Required] public required string Storage { get; set; }
    [Required] public required string Color { get; set; }

    public bool HasTradeIn { get; set; }
    public bool ExtendedCoverage { get; set; }

    [Required] public required string PaymentOption { get; set; }

    // Cross-sell
    public bool AddSmartwatch { get; set; }
    public int? SmartwatchQty { get; set; } = 1;
    public bool AddBuds { get; set; }
    public int? BudsQty { get; set; } = 1;
    public bool AddCharger { get; set; }
    public int? ChargerQty { get; set; } = 1;

    // Customer
    [Required] public int ClientId { get; set; }
    [Required, StringLength(100)] public required string FullName { get; set; }
    [Required, EmailAddress] public required string Email { get; set; } 
    [Required, Phone] public required string Phone { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}