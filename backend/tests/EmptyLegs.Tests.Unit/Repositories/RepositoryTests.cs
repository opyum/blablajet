using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using EmptyLegs.Infrastructure.Data;
using EmptyLegs.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmptyLegs.Tests.Unit.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly EmptyLegsDbContext _context;
    private readonly Repository<User> _userRepository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EmptyLegsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EmptyLegsDbContext(options);
        _userRepository = new Repository<User>(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Entity_And_Return_It()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = UserRole.Customer
        };

        // Act
        var result = await _userRepository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Email.Should().Be("test@example.com");
        
        var savedUser = await _context.Users.FindAsync(result.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Entity_When_Exists()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_Not_Exists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _userRepository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Entities()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
            new() { Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" },
            new() { Email = "user3@example.com", FirstName = "Bob", LastName = "Wilson" }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(u => u.Email == "user1@example.com");
        result.Should().Contain(u => u.Email == "user2@example.com");
        result.Should().Contain(u => u.Email == "user3@example.com");
    }

    [Fact]
    public async Task FindAsync_Should_Return_Entities_Matching_Predicate()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Email = "john@example.com", FirstName = "John", LastName = "Doe", Role = UserRole.Customer },
            new() { Email = "jane@example.com", FirstName = "Jane", LastName = "Smith", Role = UserRole.Company },
            new() { Email = "bob@example.com", FirstName = "Bob", LastName = "Wilson", Role = UserRole.Customer }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.FindAsync(u => u.Role == UserRole.Customer);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "john@example.com");
        result.Should().Contain(u => u.Email == "bob@example.com");
        result.Should().NotContain(u => u.Email == "jane@example.com");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_Should_Return_First_Matching_Entity()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Email = "john@example.com", FirstName = "John", LastName = "Doe" },
            new() { Email = "jane@example.com", FirstName = "Jane", LastName = "Smith" }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.FirstOrDefaultAsync(u => u.FirstName == "John");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("john@example.com");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_Should_Return_Null_When_No_Match()
    {
        // Arrange
        var user = new User
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.FirstOrDefaultAsync(u => u.FirstName == "NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AnyAsync_Should_Return_True_When_Match_Exists()
    {
        // Arrange
        var user = new User
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.AnyAsync(u => u.Email == "john@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_Should_Return_False_When_No_Match()
    {
        // Arrange
        var user = new User
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.AnyAsync(u => u.Email == "nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_Should_Return_Total_Count_Without_Predicate()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Email = "user1@example.com", FirstName = "John", LastName = "Doe" },
            new() { Email = "user2@example.com", FirstName = "Jane", LastName = "Smith" },
            new() { Email = "user3@example.com", FirstName = "Bob", LastName = "Wilson" }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.CountAsync();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public async Task CountAsync_Should_Return_Filtered_Count_With_Predicate()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Email = "user1@example.com", FirstName = "John", LastName = "Doe", Role = UserRole.Customer },
            new() { Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Role = UserRole.Company },
            new() { Email = "user3@example.com", FirstName = "Bob", LastName = "Wilson", Role = UserRole.Customer }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.CountAsync(u => u.Role == UserRole.Customer);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public async Task Update_Should_Modify_Entity()
    {
        // Arrange
        var user = new User
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        user.FirstName = "Updated John";
        _userRepository.Update(user);
        await _context.SaveChangesAsync();

        // Assert
        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Should().Be("Updated John");
    }

    [Fact]
    public async Task SoftDeleteAsync_Should_Mark_Entity_As_Deleted()
    {
        // Arrange
        var user = new User
        {
            Email = "john@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _userRepository.SoftDeleteAsync(user.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedUser = await _context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == user.Id);
        deletedUser.Should().NotBeNull();
        deletedUser!.IsDeleted.Should().BeTrue();
        deletedUser.DeletedAt.Should().NotBeNull();
        deletedUser.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify the soft-deleted entity is not returned by normal queries (due to global query filter)
        var nonDeletedUser = await _userRepository.GetByIdAsync(user.Id);
        nonDeletedUser.Should().BeNull();
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Correct_Page()
    {
        // Arrange
        var users = new List<User>();
        for (int i = 1; i <= 10; i++)
        {
            users.Add(new User
            {
                Email = $"user{i}@example.com",
                FirstName = $"User{i}",
                LastName = "Doe"
            });
        }

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetPagedAsync(2, 3); // Page 2, 3 items per page

        // Assert
        result.Should().HaveCount(3);
        // Note: Exact ordering depends on database provider behavior
        // This test verifies pagination works, but specific items depend on ordering
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}