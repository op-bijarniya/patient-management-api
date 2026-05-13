using PatientManagement.Domain.Entities;

namespace PatientManagement.Domain.Interfaces;

public interface IVisitRepository
{
    Task<Visit> AddAsync(Visit visit, CancellationToken cancellationToken = default);
    Task<Visit?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Visit>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
