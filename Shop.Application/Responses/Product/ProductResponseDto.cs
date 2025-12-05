namespace Shop.Application.Responses.Product;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }
}
