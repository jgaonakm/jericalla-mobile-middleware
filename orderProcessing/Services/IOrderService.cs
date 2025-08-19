using Models;
public interface IOrderService
{
    Task<long> CreateFromMessageAsync(PhoneReservation dto, CancellationToken ct = default);
}