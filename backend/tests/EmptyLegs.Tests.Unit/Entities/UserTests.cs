using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Enums;
using FluentAssertions;
using Xunit;

namespace EmptyLegs.Tests.Unit.Entities;

public class UserTests
{
    [Fact]
    public void User_FullName_Should_Combine_FirstName_And_LastName()
    {
        // Arrange
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.Should().Be("John Doe");
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("John", "")]
    [InlineData("", "Doe")]
    [InlineData(null, null)]
    public void User_FullName_Should_Handle_Empty_Or_Null_Names(string? firstName, string? lastName)
    {
        // Arrange
        var user = new User
        {
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty
        };

        // Act
        var fullName = user.FullName;

        // Assert
        fullName.Should().NotBeNull();
        fullName.Should().Be($"{firstName ?? string.Empty} {lastName ?? string.Empty}");
    }

    [Fact]
    public void User_Should_Have_Default_Values()
    {
        // Act
        var user = new User();

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.Should().Be(string.Empty);
        user.FirstName.Should().Be(string.Empty);
        user.LastName.Should().Be(string.Empty);
        user.Role.Should().Be(UserRole.Customer);
        user.IsActive.Should().BeTrue();
        user.IsEmailVerified.Should().BeFalse();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.IsDeleted.Should().BeFalse();
        user.DeletedAt.Should().BeNull();
        user.Bookings.Should().NotBeNull().And.BeEmpty();
        user.Alerts.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData(UserRole.Customer)]
    [InlineData(UserRole.Company)]
    [InlineData(UserRole.Admin)]
    public void User_Should_Accept_Valid_Roles(UserRole role)
    {
        // Arrange & Act
        var user = new User { Role = role };

        // Assert
        user.Role.Should().Be(role);
    }

    [Fact]
    public void User_Should_Allow_Optional_Company_Association()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var user = new User { CompanyId = companyId };

        // Act & Assert
        user.CompanyId.Should().Be(companyId);
        user.Company.Should().BeNull(); // Navigation property not set
    }

    [Fact]
    public void User_Should_Allow_Null_PhoneNumber_And_DateOfBirth()
    {
        // Arrange & Act
        var user = new User
        {
            PhoneNumber = null,
            DateOfBirth = null
        };

        // Assert
        user.PhoneNumber.Should().BeNull();
        user.DateOfBirth.Should().BeNull();
    }

    [Fact]
    public void User_Should_Set_Valid_PhoneNumber_And_DateOfBirth()
    {
        // Arrange
        var phoneNumber = "+33123456789";
        var dateOfBirth = new DateTime(1990, 5, 15);

        // Act
        var user = new User
        {
            PhoneNumber = phoneNumber,
            DateOfBirth = dateOfBirth
        };

        // Assert
        user.PhoneNumber.Should().Be(phoneNumber);
        user.DateOfBirth.Should().Be(dateOfBirth);
    }
}