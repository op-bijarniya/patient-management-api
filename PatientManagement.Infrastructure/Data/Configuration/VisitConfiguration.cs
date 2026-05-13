using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Data.Configuration;

public class VisitConfiguration : IEntityTypeConfiguration<Visit>
{
    public void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.VisitDate)
            .IsRequired();

        builder.Property(v => v.TemperatureC)
            .IsRequired();

        builder.Property(v => v.BloodPressure)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(v => v.Pulse)
            .IsRequired();

        builder.Property(v => v.Complaints)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(v => v.Diagnosis)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(v => v.Notes)
            .HasMaxLength(1000);

        builder.HasMany(v => v.Medications)
            .WithOne(m => m.Visit)
            .HasForeignKey(m => m.VisitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
