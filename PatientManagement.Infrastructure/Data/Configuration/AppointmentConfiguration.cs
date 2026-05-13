using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Infrastructure.Data.Configuration;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ScheduledAt)
            .IsRequired();

        builder.Property(a => a.Reason)
            .HasMaxLength(250);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired();
    }
}
