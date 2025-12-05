using Shop.Domain.Exceptions;
using Shop.Domain.Helpers;
using Shop.Domain.Models.Base;

namespace Shop.Domain.Models;

public class Product : AuditableBaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int Quantity { get; private set; }


    private Product() { }
    public static Product Create(string name, string description, int quantity)
    {
        var product = new Product();
        product.Apply(name, description, quantity);
        return product;
    }

    public void Update(string name, string description, int quantity)
    {
        Apply(name, description, quantity);
    }

    private void Apply(string name, string description, int quantity)
    {
        DomainValidatorHelper.ThrowIfNullOrWhiteSpace(name, nameof(name));
        DomainValidatorHelper.ThrowIfTooLong(name, 200, nameof(name));

        DomainValidatorHelper.ThrowIfNullOrWhiteSpace(description, nameof(description));
        DomainValidatorHelper.ThrowIfTooLong(description, 1000, nameof(description));

        if (quantity < 0)
            throw new DomainValidationException("Product quantity cannot be negative.");

        Name = name.Trim();
        Description = description.Trim();
        Quantity = quantity;
    }

}
