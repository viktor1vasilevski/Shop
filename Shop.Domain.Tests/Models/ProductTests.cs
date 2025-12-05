using FluentAssertions;
using Shop.Domain.Exceptions;
using Shop.Domain.Models;

namespace Shop.Domain.Tests.Models;

public class ProductTests
{
    [Fact]
    public void Create_ShouldReturnProduct_WhenValidInput()
    {
        // Arrange
        var name = "Product 1";
        var description = "Some description";
        var quantity = 5;

        // Act
        var product = Product.Create(name, description, quantity);

        // Assert
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Quantity.Should().Be(quantity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_ShouldThrow_WhenNameIsNullOrWhitespace(string invalidName)
    {
        // Act
        Action act = () => Product.Create(invalidName, "Valid desc", 1);

        // Assert
        act.Should().Throw<DomainValidationException>()
           .WithMessage("*name*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_ShouldThrow_WhenDescriptionIsNullOrWhitespace(string invalidDesc)
    {
        // Act
        Action act = () => Product.Create("Valid name", invalidDesc, 1);

        // Assert
        act.Should().Throw<DomainValidationException>()
           .WithMessage("*description*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenQuantityIsNegative()
    {
        // Act
        Action act = () => Product.Create("Valid name", "Valid desc", -1);

        // Assert
        act.Should().Throw<DomainValidationException>()
           .WithMessage("Product quantity cannot be negative.");
    }


    [Fact]
    public void Update_ShouldChangeValues_WhenValidInput()
    {
        // Arrange
        var product = Product.Create("Old name", "Old desc", 5);

        // Act
        product.Update("New name", "New desc", 10);

        // Assert
        product.Name.Should().Be("New name");
        product.Description.Should().Be("New desc");
        product.Quantity.Should().Be(10);
    }

    [Fact]
    public void Update_ShouldThrow_WhenQuantityIsNegative()
    {
        // Arrange
        var product = Product.Create("Name", "Desc", 5);

        // Act
        Action act = () => product.Update("Name", "Desc", -5);

        // Assert
        act.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Create_ShouldTrimNameAndDescription()
    {
        // Arrange
        var name = "  Product Name  ";
        var description = "  Product Description  ";
        var quantity = 5;

        // Act
        var product = Product.Create(name, description, quantity);

        // Assert
        product.Name.Should().Be("Product Name");
        product.Description.Should().Be("Product Description");
    }

    [Fact]
    public void Update_ShouldTrimNameAndDescription()
    {
        // Arrange
        var product = Product.Create("Old Name", "Old Desc", 5);

        // Act
        product.Update("  New Name  ", "  New Desc  ", 10);

        // Assert
        product.Name.Should().Be("New Name");
        product.Description.Should().Be("New Desc");
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 201); // max 200
        var description = "Valid Desc";

        // Act
        Action act = () => Product.Create(longName, description, 1);

        // Assert
        act.Should().Throw<DomainValidationException>()
           .WithMessage("*name*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var name = "Valid Name";
        var longDesc = new string('a', 1001); // max 1000

        // Act
        Action act = () => Product.Create(name, longDesc, 1);

        // Assert
        act.Should().Throw<DomainValidationException>()
           .WithMessage("*description*");
    }

    [Fact]
    public void Update_ShouldThrow_WhenNameExceedsMaxLength()
    {
        var product = Product.Create("Valid Name", "Valid Desc", 5);
        var longName = new string('a', 201);

        Action act = () => product.Update(longName, "Updated Desc", 5);

        act.Should().Throw<DomainValidationException>()
           .WithMessage("*name*");
    }

    [Fact]
    public void Update_ShouldThrow_WhenDescriptionExceedsMaxLength()
    {
        var product = Product.Create("Valid Name", "Valid Desc", 5);
        var longDesc = new string('a', 1001);

        Action act = () => product.Update("Updated Name", longDesc, 5);

        act.Should().Throw<DomainValidationException>()
           .WithMessage("*description*");
    }

}
