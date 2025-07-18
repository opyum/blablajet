using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Interfaces;
using EmptyLegs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace EmptyLegs.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EmptyLegsDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(EmptyLegsDbContext context)
    {
        _context = context;
        
        // Initialize repositories
        Users = new Repository<User>(_context);
        Companies = new Repository<Company>(_context);
        Aircraft = new Repository<Aircraft>(_context);
        Airports = new Repository<Airport>(_context);
        Flights = new Repository<Flight>(_context);
        Bookings = new Repository<Booking>(_context);
        Passengers = new Repository<Passenger>(_context);
        Payments = new Repository<Payment>(_context);
        Documents = new Repository<Document>(_context);
        Reviews = new Repository<Review>(_context);
        UserAlerts = new Repository<UserAlert>(_context);
        BookingServices = new Repository<BookingService>(_context);
    }

    public IRepository<User> Users { get; private set; }
    public IRepository<Company> Companies { get; private set; }
    public IRepository<Aircraft> Aircraft { get; private set; }
    public IRepository<Airport> Airports { get; private set; }
    public IRepository<Flight> Flights { get; private set; }
    public IRepository<Booking> Bookings { get; private set; }
    public IRepository<Passenger> Passengers { get; private set; }
    public IRepository<Payment> Payments { get; private set; }
    public IRepository<Document> Documents { get; private set; }
    public IRepository<Review> Reviews { get; private set; }
    public IRepository<UserAlert> UserAlerts { get; private set; }
    public IRepository<BookingService> BookingServices { get; private set; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}