using Data.Entities;
public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _db;
    public OrderRepository(OrderDbContext db) => _db = db;

    public Task AddAsync(PhoneReservation entity, CancellationToken ct = default)
        => _db.PhoneReservations.AddAsync(entity, ct).AsTask();
}
