using EmptyLegs.Core.Entities;
using EmptyLegs.Infrastructure.Data;
using EmptyLegs.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmptyLegs.Tests.Unit.Infrastructure;

public class RepositoryTests : IDisposable
{
    private readonly EmptyLegsDbContext _context;
    private readonly Repository<Flight> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EmptyLegsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EmptyLegsDbContext(options);
        _repository = new Repository<Flight>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityToDatabase()
    {
        // Arrange
        var flight = new Flight
        {
            FlightNumber = "TEST001",
            BasePrice = 2500m,
            CurrentPrice = 2000m,
            AvailableSeats = 6,
            TotalSeats = 6
        };

        // Act
        var result = await _repository.AddAsync(flight);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.FlightNumber.Should().Be("TEST001");

        var savedFlight = await _context.Flights.FindAsync(result.Id);
        savedFlight.Should().NotBeNull();
        savedFlight!.FlightNumber.Should().Be("TEST001");
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
    {
        // Arrange
        var flight = new Flight
        {
            FlightNumber = "TEST002",
            BasePrice = 3000m,
            CurrentPrice = 2500m,
            AvailableSeats = 8,
            TotalSeats = 8
        };

        await _repository.AddAsync(flight);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(flight.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(flight.Id);
        result.FlightNumber.Should().Be("TEST002");
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedEntities()
    {
        // Arrange
        var flight1 = new Flight { FlightNumber = "TEST003", BasePrice = 2000m, CurrentPrice = 1800m, AvailableSeats = 4, TotalSeats = 4 };
        var flight2 = new Flight { FlightNumber = "TEST004", BasePrice = 2500m, CurrentPrice = 2200m, AvailableSeats = 6, TotalSeats = 6 };
        var flight3 = new Flight { FlightNumber = "TEST005", BasePrice = 3000m, CurrentPrice = 2800m, AvailableSeats = 8, TotalSeats = 8, IsDeleted = true };

        await _repository.AddAsync(flight1);
        await _repository.AddAsync(flight2);
        await _repository.AddAsync(flight3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2); // Only non-deleted entities
        result.Should().Contain(f => f.FlightNumber == "TEST003");
        result.Should().Contain(f => f.FlightNumber == "TEST004");
        result.Should().NotContain(f => f.FlightNumber == "TEST005"); // Deleted entity
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ShouldReturnMatchingEntities()
    {
        // Arrange
        var flight1 = new Flight { FlightNumber = "CDG001", BasePrice = 2000m, CurrentPrice = 1800m, AvailableSeats = 4, TotalSeats = 4 };
        var flight2 = new Flight { FlightNumber = "CDG002", BasePrice = 2500m, CurrentPrice = 2200m, AvailableSeats = 6, TotalSeats = 6 };
        var flight3 = new Flight { FlightNumber = "ORY001", BasePrice = 3000m, CurrentPrice = 2800m, AvailableSeats = 8, TotalSeats = 8 };

        await _repository.AddAsync(flight1);
        await _repository.AddAsync(flight2);
        await _repository.AddAsync(flight3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(f => f.FlightNumber.StartsWith("CDG"));

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.FlightNumber == "CDG001");
        result.Should().Contain(f => f.FlightNumber == "CDG002");
        result.Should().NotContain(f => f.FlightNumber == "ORY001");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithPredicate_ShouldReturnFirstMatch()
    {
        // Arrange
        var flight1 = new Flight { FlightNumber = "FIRST001", BasePrice = 2000m, CurrentPrice = 1800m, AvailableSeats = 4, TotalSeats = 4 };
        var flight2 = new Flight { FlightNumber = "FIRST002", BasePrice = 2500m, CurrentPrice = 2200m, AvailableSeats = 6, TotalSeats = 6 };

        await _repository.AddAsync(flight1);
        await _repository.AddAsync(flight2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FirstOrDefaultAsync(f => f.FlightNumber.StartsWith("FIRST"));

        // Assert
        result.Should().NotBeNull();
        result!.FlightNumber.Should().Be("FIRST001"); // Should return the first one
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_ShouldReturnTotalCount()
    {
        // Arrange
        var flight1 = new Flight { FlightNumber = "COUNT001", BasePrice = 2000m, CurrentPrice = 1800m, AvailableSeats = 4, TotalSeats = 4 };
        var flight2 = new Flight { FlightNumber = "COUNT002", BasePrice = 2500m, CurrentPrice = 2200m, AvailableSeats = 6, TotalSeats = 6 };
        var flight3 = new Flight { FlightNumber = "COUNT003", BasePrice = 3000m, CurrentPrice = 2800m, AvailableSeats = 8, TotalSeats = 8, IsDeleted = true };

        await _repository.AddAsync(flight1);
        await _repository.AddAsync(flight2);
        await _repository.AddAsync(flight3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountAsync();

        // Assert
        result.Should().Be(2); // Only non-deleted entities
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        // Arrange
        var flight1 = new Flight { FlightNumber = "PRED001", BasePrice = 1500m, CurrentPrice = 1300m, AvailableSeats = 4, TotalSeats = 4 };
        var flight2 = new Flight { FlightNumber = "PRED002", BasePrice = 2500m, CurrentPrice = 2200m, AvailableSeats = 6, TotalSeats = 6 };
        var flight3 = new Flight { FlightNumber = "PRED003", BasePrice = 3500m, CurrentPrice = 3200m, AvailableSeats = 8, TotalSeats = 8 };

        await _repository.AddAsync(flight1);
        await _repository.AddAsync(flight2);
        await _repository.AddAsync(flight3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountAsync(f => f.BasePrice > 2000m);

        // Assert
        result.Should().Be(2); // flight2 and flight3
    }

    [Fact]
    public async Task SoftDeleteAsync_ShouldMarkEntityAsDeleted()
    {
        // Arrange
        var flight = new Flight
        {
            FlightNumber = "DELETE001",
            BasePrice = 2500m,
            CurrentPrice = 2200m,
            AvailableSeats = 6,
            TotalSeats = 6
        };

        await _repository.AddAsync(flight);
        await _context.SaveChangesAsync();

        // Act
        await _repository.SoftDeleteAsync(flight.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedFlight = await _context.Flights.IgnoreQueryFilters().FirstAsync(f => f.Id == flight.Id);
        deletedFlight.IsDeleted.Should().BeTrue();
        deletedFlight.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify entity is not returned by normal queries
        var result = await _repository.GetByIdAsync(flight.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var flight = new Flight
            {
                FlightNumber = $"PAGE{i:000}",
                BasePrice = 2000m + i * 100,
                CurrentPrice = 1800m + i * 100,
                AvailableSeats = 4 + i,
                TotalSeats = 4 + i
            };
            await _repository.AddAsync(flight);
        }
        await _context.SaveChangesAsync();

        // Act
        var page1 = await _repository.GetPagedAsync(1, 3); // First 3 items
        var page2 = await _repository.GetPagedAsync(2, 3); // Next 3 items

        // Assert
        page1.Should().HaveCount(3);
        page2.Should().HaveCount(3);
        
        // Verify no overlap
        var page1Ids = page1.Select(f => f.Id).ToList();
        var page2Ids = page2.Select(f => f.Id).ToList();
        page1Ids.Should().NotIntersectWith(page2Ids);
    }

    [Fact]
    public async Task Update_ShouldModifyEntity()
    {
        // Arrange
        var flight = new Flight
        {
            FlightNumber = "UPDATE001",
            BasePrice = 2500m,
            CurrentPrice = 2200m,
            AvailableSeats = 6,
            TotalSeats = 6
        };

        await _repository.AddAsync(flight);
        await _context.SaveChangesAsync();

        // Act
        flight.CurrentPrice = 1900m;
        flight.AvailableSeats = 5;
        _repository.Update(flight);
        await _context.SaveChangesAsync();

        // Assert
        var updatedFlight = await _repository.GetByIdAsync(flight.Id);
        updatedFlight.Should().NotBeNull();
        updatedFlight!.CurrentPrice.Should().Be(1900m);
        updatedFlight.AvailableSeats.Should().Be(5);
        updatedFlight.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}