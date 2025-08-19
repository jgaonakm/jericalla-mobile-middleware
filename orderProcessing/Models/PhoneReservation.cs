using System.ComponentModel.DataAnnotations;
namespace Models;

public class PhoneReservation
{
    [Required] public PlanType PlanType { get; set; } = PlanType.Unlocked;

    [Required] public string Storage { get; set; } = "GB512";

    [Required] public string? Color { get; set; }

    public bool HasTradeIn { get; set; }
    public bool ExtendedCoverage { get; set; } = false;

    [Required] public PaymentOption PaymentOption { get; set; } = PaymentOption.Single;

    // Cross-sell
    public bool AddSmartwatch { get; set; }
    public int? SmartwatchQty { get; set; } = 1;
    public bool AddBuds { get; set; }
    public int? BudsQty { get; set; } = 1;
    public bool AddCharger { get; set; }
    public int? ChargerQty { get; set; } = 1;

    // Customer
    [Required] public int ClientId { get; set; }
    [Required, StringLength(100)] public string FullName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, Phone] public string Phone { get; set; } = "";
}

public enum PlanType { Unlocked, Contract24 }
public enum PaymentOption { Single, Installments }