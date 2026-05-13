using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Data.Configuration;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Gender)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasMaxLength(150);

        builder.Property(p => p.Address)
            .HasMaxLength(250);

        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Visits)
            .WithOne(v => v.Patient)
            .HasForeignKey(v => v.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
