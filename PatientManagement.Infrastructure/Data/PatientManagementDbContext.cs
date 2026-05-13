using Microsoft.EntityFrameworkCore;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data.Configuration;

namespace PatientManagement.Infrastructure.Data;

public class PatientManagementDbContext : DbContext
{
    public PatientManagementDbContext(DbContextOptions<PatientManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
        modelBuilder.ApplyConfiguration(new VisitConfiguration());
        modelBuilder.ApplyConfiguration(new MedicationConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
