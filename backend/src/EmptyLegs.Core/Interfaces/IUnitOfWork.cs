using EmptyLegs.Core.Entities;

namespace EmptyLegs.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Company> Companies { get; }
    IRepository<Aircraft> Aircraft { get; }
    IRepository<Airport> Airports { get; }
    IRepository<Flight> Flights { get; }
    IRepository<Booking> Bookings { get; }
    IRepository<Passenger> Passengers { get; }
    IRepository<Payment> Payments { get; }
    IRepository<Document> Documents { get; }
    IRepository<Review> Reviews { get; }
    IRepository<UserAlert> UserAlerts { get; }
    IRepository<BookingService> BookingServices { get; }
    IRepository<RefreshToken> RefreshTokens { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}