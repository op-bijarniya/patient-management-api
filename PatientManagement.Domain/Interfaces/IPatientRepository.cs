using PatientManagement.Domain.Entities;

namespace PatientManagement.Domain.Interfaces;

public interface IPatientRepository
{
    Task<Patient> AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Patient>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Patient>> SearchAsync(string? searchText, CancellationToken cancellationToken = default);
    Task<Patient> UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Patient>> GetRecentPatientsAsync(int count, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
