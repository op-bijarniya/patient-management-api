using Microsoft.EntityFrameworkCore;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly PatientManagementDbContext _dbContext;

    public AppointmentRepository(PatientManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Appointments.AddAsync(appointment, cancellationToken);
        return appointment;
    }

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Appointment>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments
            .Where(a => a.ScheduledAt.Date == date.Date)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        _dbContext.Appointments.Update(appointment);
        return appointment;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
