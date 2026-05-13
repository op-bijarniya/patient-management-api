using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Data.Configuration;

public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(m => m.Dosage)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Frequency)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Duration)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Instructions)
            .HasMaxLength(1000);
    }
}
