using Data.Entities;
public interface IOrderRepository
{
    Task AddAsync(PhoneReservation entity, CancellationToken ct = default);
}