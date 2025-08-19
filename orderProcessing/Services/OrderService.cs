using System.ComponentModel.DataAnnotations;
using Models;
using PhoneReservationEntity = Data.Entities.PhoneReservation;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository repo,
        IUnitOfWork uow,
        ILogger<OrderService> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<long> CreateFromMessageAsync(Models.PhoneReservation dto, CancellationToken ct = default)
    {
        // Validate using DataAnnotations already present on your DTO
        var ctx = new ValidationContext(dto);
        Validator.ValidateObject(dto, ctx, validateAllProperties: true);

        // Map DTO -> Entity (manual mapping stub)
        var entity = new PhoneReservationEntity
        {
            PlanType = dto.PlanType == PlanType.Unlocked ? "Unlocked" : "Contract24",
            Storage = dto.Storage,
            Color = dto.Color ?? "Any",
            HasTradeIn = dto.HasTradeIn,
            ExtendedCoverage = dto.ExtendedCoverage,
            PaymentOption = dto.PaymentOption == PaymentOption.Single ? "Single" : "Installments",
            // cross-sell items
            AddSmartwatch = dto.AddSmartwatch,
            SmartwatchQty = dto.AddSmartwatch ? dto.SmartwatchQty : null,
            AddBuds = dto.AddBuds,
            BudsQty = dto.AddBuds ? dto.BudsQty : null,
            AddCharger = dto.AddCharger,
            ChargerQty = dto.AddCharger ? dto.ChargerQty : null,
            //customer info
            ClientId = dto.ClientId,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Stored reservation {Id} for client {ClientId}", entity.Id, entity.ClientId);
        return entity.Id;
    }
}
