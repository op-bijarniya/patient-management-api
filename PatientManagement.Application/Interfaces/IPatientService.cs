using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;

namespace PatientManagement.Application.Interfaces;

public interface IPatientService
{
    Task<PatientDto> CreatePatientAsync(CreatePatientCommand command, CancellationToken cancellationToken = default);
    Task<PatientDto> UpdatePatientAsync(UpdatePatientCommand command, CancellationToken cancellationToken = default);
    Task<PatientDto?> GetPatientAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PatientDto>> SearchPatientsAsync(string? searchText, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PatientDto>> GetRecentPatientsAsync(int count, CancellationToken cancellationToken = default);
}
