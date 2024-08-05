using DigiBuy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DigiBuy.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);

        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        
        builder.Property(u => u.WalletBalance)
            .HasColumnType("decimal(18,2)").HasDefaultValue(0);

        builder.Property(u => u.PointsBalance)
            .HasColumnType("decimal(18,2)").HasDefaultValue(0);
        
        builder.Property(u => u.Status).HasConversion<string>().IsRequired();
    }
}