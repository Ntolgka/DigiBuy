﻿using DigiBuy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiBuy.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(o => o.CouponAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(o => o.PointsUsed).IsRequired().HasColumnType("decimal(18,2)");

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        builder.HasMany(o => o.OrderDetails)
            .WithOne(od => od.Order)
            .HasForeignKey(od => od.OrderId);
    }
}