using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Shop.Application.Constants;
using Shop.Application.Enums;
using Shop.Application.Requests;
using Shop.Application.Services;
using Shop.Domain.Interfaces;
using Shop.Domain.Models;

namespace Shop.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IEfRepository<Product>> _productRepositoryMock;
    private readonly Mock<IEfUnitOfWork> _uowMock;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IEfRepository<Product>>();
        _uowMock = new Mock<IEfUnitOfWork>();
        _sut = new ProductService(_uowMock.Object, _productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 5),
            Product.Create("Product 2", "Description 2", 10)
        };

        _productRepositoryMock
            .Setup(r => r.GetAllAsync(asNoTracking: true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _sut.GetProductsAsync();

        // Assert
        result.Status.Should().Be(ResponseStatus.Success);
        result.Data.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("Product 1");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenFound()
    {
        var product = Product.Create("Product 1", "Description 1", 5);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, asNoTracking: true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _sut.GetProductByIdAsync(product.Id);

        result.Status.Should().Be(ResponseStatus.Success);
        result.Data!.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var id = Guid.NewGuid();

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(id, asNoTracking: true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _sut.GetProductByIdAsync(id);

        result.Status.Should().Be(ResponseStatus.NotFound);
        result.Message.Should().Be(ProductConstants.ProductWithIdNotFound);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCreateProduct_WhenNameIsUnique()
    {
        var request = new CreateProductRequest
        {
            Name = "New Product",
            Description = "Desc",
            Quantity = 5
        };

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), asNoTracking: true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.CreateProductAsync(request);

        result.Status.Should().Be(ResponseStatus.Created);
        result.Message.Should().Be(ProductConstants.ProductSuccessfullyCreated);
        result.Data!.Name.Should().Be("New Product");
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnConflict_WhenNameExists()
    {
        var request = new CreateProductRequest { Name = "Existing", Description = "Desc", Quantity = 5 };

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), asNoTracking: true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.CreateProductAsync(request);

        result.Status.Should().Be(ResponseStatus.Conflict);
        result.Message.Should().Be(ProductConstants.ProductExist);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
    {
        var product = Product.Create("Old Name", "Old Desc", 3);
        var request = new UpdateProductRequest { Name = "Updated", Description = "Updated Desc", Quantity = 10 };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, asNoTracking: false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.UpdateProductAsync(product.Id, request);
        
        result.Status.Should().Be(ResponseStatus.Updated);
        result.Message.Should().Be(ProductConstants.ProductSuccessfullyUpdated);
        result.Data!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var request = new UpdateProductRequest { Name = "Updated", Description = "Updated Desc", Quantity = 10 };
        var id = Guid.NewGuid();

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(id, asNoTracking: false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _sut.UpdateProductAsync(id, request);

        result.Status.Should().Be(ResponseStatus.NotFound);
        result.Message.Should().Be(ProductConstants.ProductWithIdNotFound);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
    {
        var product = Product.Create("ToDelete", "Desc", 5);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, asNoTracking: false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _sut.DeleteProductAsync(product.Id);

        result.Status.Should().Be(ResponseStatus.Success);
        result.Message.Should().Be(ProductConstants.ProductSuccessfullyDeleted);
        _productRepositoryMock.Verify(r => r.Delete(product), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var id = Guid.NewGuid();

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(id, asNoTracking: false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _sut.DeleteProductAsync(id);

        result.Status.Should().Be(ResponseStatus.NotFound);
        result.Message.Should().Be(ProductConstants.ProductWithIdNotFound);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnBadRequest_WhenDomainValidationFails()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "",
            Description = "Desc",
            Quantity = 5
        };

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), asNoTracking: true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateProductAsync(request);

        // Assert
        result.Status.Should().Be(ResponseStatus.BadRequest);
        result.Message.Should().Be("name cannot be empty.");
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnBadRequest_WhenDomainValidationFails()
    {
        // Arrange
        var product = Product.Create("Valid Name", "Valid Description", 5);
        var request = new UpdateProductRequest
        {
            Name = "",
            Description = "Updated Description",
            Quantity = 10
        };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, asNoTracking: false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.UpdateProductAsync(product.Id, request);

        // Assert
        result.Status.Should().Be(ResponseStatus.BadRequest);
        result.Message.Should().Be("name cannot be empty.");
    }


}
