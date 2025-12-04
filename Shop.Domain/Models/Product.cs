using Shop.Domain.Models.Base;

namespace Shop.Domain.Models;

public class Product : AuditableBaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }

}
