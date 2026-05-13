using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.HasIndex(u => u.Username)
            .IsUnique();
    }
}
