using PatientManagement.Domain.Entities;

namespace PatientManagement.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Appointment>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<Appointment> UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
