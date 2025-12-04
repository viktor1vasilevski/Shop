namespace Shop.Domain.Models.Base;

public abstract class AuditableBaseEntity : BaseEntity
{
    public virtual string CreatedBy { get; set; } = null!;
    public virtual DateTime Created { get; set; }
    public virtual string? LastModifiedBy { get; set; }
    public virtual DateTime? LastModified { get; set; }
}
