using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shop.Domain.Models;

namespace Shop.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Product_Quantity_NonNegative", "[Quantity] >= 0");
        });

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.HasIndex(p => p.Name).IsUnique();

        builder.Property(p => p.Description)
               .HasMaxLength(1000);

        builder.Property(p => p.Quantity)
               .IsRequired()
               .HasDefaultValue(0);

        builder.Property(p => p.Created)
               .IsRequired();

        builder.Property(p => p.CreatedBy)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(p => p.LastModified)
               .IsRequired(false);

        builder.Property(p => p.LastModifiedBy)
               .HasMaxLength(256)
               .IsRequired(false);
    }
}
