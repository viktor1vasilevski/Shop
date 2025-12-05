using FluentAssertions;
using Moq;
using Shop.Application.Constants;
using Shop.Application.Enums;
using Shop.Application.Requests;
using Shop.Application.Services;
using Shop.Domain.Interfaces;
using Shop.Domain.Models;
using System.Linq.Expressions;

namespace Shop.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IEfRepository<Product>> _productRepositoryMock;
    private readonly Mock<IEfUnitOfWork> _uowMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IEfRepository<Product>>();
        _uowMock = new Mock<IEfUnitOfWork>();
        _productService = new ProductService(_uowMock.Object, _productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetProductsAsync_ShouldReturnAllProducts()
    {
        var productsInDb = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 5),
            Product.Create("Product 2", "Description 2", 10)
        };

        _productRepositoryMock
            .Setup(r => r.GetAllAsync(asNoTracking: true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(productsInDb);

        var result = await _productService.GetProductsAsync();

        result.Status.Should().Be(ResponseStatus.Success);
        result.Data.Should().HaveCount(2);
        result.Data[0].Name.Should().Be("Product 1");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenFound()
    {
        var productsInDb = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 5)
        };
        var product = productsInDb[0];

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, bool _, CancellationToken __) => productsInDb.FirstOrDefault(p => p.Id == id));

        var result = await _productService.GetProductByIdAsync(product.Id);

        result.Status.Should().Be(ResponseStatus.Success);
        result.Data!.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _productService.GetProductByIdAsync(Guid.NewGuid());

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
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _productService.CreateProductAsync(request);

        result.Status.Should().Be(ResponseStatus.Created);
        result.Message.Should().Be(ProductConstants.ProductSuccessfullyCreated);
        result.Data!.Name.Should().Be("New Product");

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnConflict_WhenNameExists()
    {
        var request = new CreateProductRequest { Name = "Existing", Description = "Desc", Quantity = 5 };

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _productService.CreateProductAsync(request);

        result.Status.Should().Be(ResponseStatus.Conflict);
        result.Message.Should().Be(ProductConstants.ProductExist);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnBadRequest_WhenDomainValidationFails()
    {
        var request = new CreateProductRequest
        {
            Name = "", // invalid name
            Description = "Desc",
            Quantity = 5
        };

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _productService.CreateProductAsync(request);

        result.Status.Should().Be(ResponseStatus.BadRequest);
        result.Message.Should().Be("name cannot be empty.");
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
    {
        var productToUpdate = Product.Create("Old Name", "Old Desc", 3);
        var request = new UpdateProductRequest { Name = "Updated", Description = "Updated Desc", Quantity = 10 };

        var productsInDb = new List<Product> { productToUpdate };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, bool _, CancellationToken __) => productsInDb.FirstOrDefault(p => p.Id == id));

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Product, bool>> predicate, bool _, CancellationToken __) =>
            {
                var func = predicate.Compile();
                return productsInDb.Any(func);
            });

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _productService.UpdateProductAsync(productToUpdate.Id, request);

        result.Status.Should().Be(ResponseStatus.Updated);
        result.Message.Should().Be(ProductConstants.ProductSuccessfullyUpdated);
        result.Data!.Name.Should().Be("Updated");

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var request = new UpdateProductRequest { Name = "Updated", Description = "Updated Desc", Quantity = 10 };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _productService.UpdateProductAsync(Guid.NewGuid(), request);

        result.Status.Should().Be(ResponseStatus.NotFound);
        result.Message.Should().Be(ProductConstants.ProductWithIdNotFound);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnConflict_WhenAnotherProductHasSameName()
    {
        var productToUpdate = Product.Create("Old Name", "Old Desc", 3);
        var otherProduct = Product.Create("Duplicate Name", "Some Desc", 5);

        var request = new UpdateProductRequest { Name = "Duplicate Name", Description = "Updated Desc", Quantity = 10 };

        var productsInDb = new List<Product> { productToUpdate, otherProduct };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, bool _, CancellationToken __) => productsInDb.FirstOrDefault(p => p.Id == id));

        _productRepositoryMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Product, bool>>>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Product, bool>> predicate, bool _, CancellationToken __) =>
            {
                var func = predicate.Compile();
                return productsInDb.Any(func);
            });

        var result = await _productService.UpdateProductAsync(productToUpdate.Id, request);

        result.Status.Should().Be(ResponseStatus.Conflict);
        result.Message.Should().Be(ProductConstants.ProductExist);

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateProductAsync_ShouldReturnBadRequest_WhenDomainValidationFails()
    {
        var product = Product.Create("Valid Name", "Valid Description", 5);
        var request = new UpdateProductRequest { Name = "", Description = "Updated Desc", Quantity = 10 };

        var productsInDb = new List<Product> { product };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, bool _, CancellationToken __) => productsInDb.FirstOrDefault(p => p.Id == id));

        var result = await _productService.UpdateProductAsync(product.Id, request);

        result.Status.Should().Be(ResponseStatus.BadRequest);
        result.Message.Should().Be("name cannot be empty.");
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldDeleteProduct_WhenProductExists()
    {
        var product = Product.Create("ToDelete", "Desc", 5);

        var productsInDb = new List<Product> { product };

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, bool _, CancellationToken __) => productsInDb.FirstOrDefault(p => p.Id == id));

        _productRepositoryMock.Setup(r => r.Delete(It.IsAny<Product>())).Callback<Product>(p => productsInDb.Remove(p));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _productService.DeleteProductAsync(product.Id);

        result.Status.Should().Be(ResponseStatus.Success);
        result.Message.Should().Be(ProductConstants.ProductSuccessfullyDeleted);
        productsInDb.Should().BeEmpty();
        _productRepositoryMock.Verify(r => r.Delete(product), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _productService.DeleteProductAsync(Guid.NewGuid());

        result.Status.Should().Be(ResponseStatus.NotFound);
        result.Message.Should().Be(ProductConstants.ProductWithIdNotFound);
    }
}
