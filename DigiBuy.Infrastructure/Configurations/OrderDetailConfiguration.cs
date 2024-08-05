using DigiBuy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiBuy.Infrastructure.Configurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(od => od.Id);
        builder.Property(od => od.Quantity).IsRequired();
        builder.Property(od => od.Price).IsRequired().HasColumnType("decimal(18,2)");

        builder.HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        builder.HasOne(od => od.Product)
            .WithMany()
            .HasForeignKey(od => od.ProductId);
    }
}